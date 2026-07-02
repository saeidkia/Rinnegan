using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RinneganCore
{
    public class Rinnegan<TRequest, TResponse> : IRinnegan<TRequest, TResponse>
    {
        protected TRequest? request;
        private Cache? cache;

        protected virtual Cache? EnableCaching()
        {
            return null;
        }
        protected virtual Result<TResponse>? FromCache()
        {
            this.cache = EnableCaching();

            if (this.cache == null)
                return null;

            Result<TResponse>? result;
            this.cache.found = this.cache.TryGetValue(out result);

            return result;
        }
        protected virtual async Task<Result<TResponse>?> FromCacheAsync()
        {
            this.cache = EnableCaching();

            if (this.cache == null)
                return null;

            Result<TResponse>? result = await this.cache.GetValueAsync<Result<TResponse>>();
            this.cache.found = result != null;

            return result;
        }
        protected virtual void SaveCache(Result<TResponse>? result, Cache cache)
        {
            // cache not set
            // cache found data
            // result is null
            // result is not OK
            if (this.cache == null || this.cache.found || result == null || !result.IsSuccess)
                return;

            this.cache.Set(result);
        }
        protected virtual async Task SaveCacheAsync(Result<TResponse>? result,Cache cache)
        {
            // cache not set
            // cache found data
            // result is null
            // result is not OK
            if (this.cache == null || this.cache.found || result == null || !result.IsSuccess)
                return;

            await this.cache.SetAsync(result);
        }

        protected virtual Result<TResponse>? Validation()
        {
            if (this.request == null)
            {
                var type = typeof(TRequest);
                if (!type.IsClass || type.Name == "String")
                    return null;

                this.request = (TRequest)Activator.CreateInstance(type);
            }

            // Create a ValidationContext for the user object
            var context = new ValidationContext(this.request, null, null);

            // Create a list to store the validation results
            var validationResults = new List<ValidationResult>();

            bool isValid;
            try
            {
                // Validate the user object and its properties using the context
                isValid = Validator.TryValidateObject(this.request, context, validationResults, true);
            }
            catch
            {
                isValid = true;
            }

            if (!isValid)
                // create list of error based on validation
                return new Result<TResponse>(HttpStatusCode.BadRequest,
                                            validationResults.Select(vr => new KeyValuePair<string, object?>(vr.MemberNames.First(), vr.ErrorMessage))
                                                .ToDictionary(vr => vr.Key, vr => vr.Value));

            return null;
        }
        protected virtual Result<TResponse>? Executing()
        {
            return null;
        }
        protected virtual Result<TResponse>? Execute()
        {
            return null;
        }
        protected virtual async Task<Result<TResponse>?> ExecuteAsync()
        {
            return null;
        }
        protected virtual Result<TResponse> Executed(Result<TResponse>? result)
        {
            return result!;
        }
        protected virtual void OnSuccess(Result<TResponse> result)
        {
        }
        protected virtual void OnError(Result<TResponse> result)
        {
        }
        protected virtual void Finally(Result<TResponse> result)
        {
        }
        protected virtual Result<TResponse> UnhandledException(Exception ex)
        {
            return new Result<TResponse>(HttpStatusCode.InternalServerError);
        }


        public virtual Result<TResponse> RunExecute(TRequest? request)
        {
            this.request = request;
            Result<TResponse>? result;

            try
            {
                result = Validation() ?? Executing() ?? FromCache() ?? Execute() ?? ExecuteAsync().Result;
                SaveCache(result, this.cache);

                // executed & null
                result = Executed(result) ?? new Result<TResponse>(HttpStatusCode.NoContent);
            }
            // unhandled exception
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }

            try
            {
                // success
                if (result.IsSuccess)
                    OnSuccess(result);

                // error
                else
                    OnError(result);

                // finally
                Finally(result);
            }
            catch(Exception ex)
            {
                UnhandledException(ex);
            }

            return result;
        }
        public virtual async Task<Result<TResponse>> RunExecuteAsync(TRequest? request)
        {
            this.request = request;
            Result<TResponse>? result;

            try
            {
                result = Validation() ?? Executing() ?? await FromCacheAsync() ?? await ExecuteAsync() ?? Execute();
                await SaveCacheAsync(result, this.cache);

                // executed & null
                result = Executed(result) ?? new Result<TResponse>(HttpStatusCode.NoContent);
            }
            // unhandled exception
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }

            try
            {
                // success
                if (result.IsSuccess)
                    OnSuccess(result);

                // error
                else
                    OnError(result);

                // finally
                Finally(result);
            }
            catch(Exception ex)
            { 
                UnhandledException(ex);
            }

            return result;
        }


        protected virtual IActionResult ActionResult(Controller? controller, Result<TResponse> result)
        {
            return new ObjectResult(new ResultDto<TResponse>(result))
            {
                StatusCode = result._statusCode,
            };
        }
        public virtual IActionResult RunAsActionResult(TRequest? request, Controller? controller = null)
        {
            var result = this.RunExecute(request);

            return ActionResult(controller, result);
        }
        public virtual async Task<IActionResult> RunAsActionResultAsync(TRequest? request, Controller? controller = null)
        {
            var result = await this.RunExecuteAsync(request);

            return ActionResult(controller, result);
        }



        protected virtual Result<TResponse> OK(TResponse data)
        {
            return new Result<TResponse>(System.Net.HttpStatusCode.OK)
            {
                data = data,
            };
        }
        protected virtual Result<TResponse> BadRequest(string value, string key = "")
        {
            return new Result<TResponse>(System.Net.HttpStatusCode.BadRequest, key, value);
        }
        protected virtual Result<TResponse> Forbidden(string value, string key = "")
        {
            return new Result<TResponse>(System.Net.HttpStatusCode.Forbidden, key, value);
        }
        protected virtual Result<TResponse> NotFound(string value, string key = "")
        {
            return new Result<TResponse>(System.Net.HttpStatusCode.NotFound, key, value);
        }
        protected virtual Result<TResponse> UnprocessableContent(string value, string key = "")
        {
            return new Result<TResponse>(HttpStatusCode.UnprocessableContent, key, value);
        }
        protected virtual Result<TResponse> BadGateway(string value, string key = "")
        {
            return new Result<TResponse>(System.Net.HttpStatusCode.BadGateway, key, value);
        }
        protected virtual Result<TResponse> Error(Result result)
        {
            return new Result<TResponse>(result.GetStatus())
            {
                errors = result.errors,
            };
        }

        protected virtual Result Validate<T>(T model)
        {
            if (model == null)
            {
                var type = typeof(T);
                if (!type.IsClass || type.Name == "String")
                    return null;

                model = (T)Activator.CreateInstance(type);
            }

            // Create a ValidationContext for the user object
            var context = new ValidationContext(model, null, null);

            // Create a list to store the validation results
            var validationResults = new List<ValidationResult>();

            bool isValid;
            try
            {
                // Validate the user object and its properties using the context
                isValid = Validator.TryValidateObject(model, context, validationResults, true);
            }
            catch
            {
                isValid = true;
            }

            if (!isValid)
                // create list of error based on validation
                return new Result<TResponse>(HttpStatusCode.BadRequest,
                                            validationResults.Select(vr => new KeyValuePair<string, object?>(vr.MemberNames.First(), vr.ErrorMessage))
                                                .ToDictionary(vr => vr.Key, vr => vr.Value));

            return new Result(HttpStatusCode.OK);
        }
    }
}
