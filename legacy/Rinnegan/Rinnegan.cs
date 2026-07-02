using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Rinnegan
{
    public class Rinnegan<TRequest, TResponse>
    {
        protected TRequest request;
        private Cache cache;


        protected Result<TResponse> OK(TResponse data)
        {
            return new Result<TResponse>(HttpStatusCode.OK)
            {
                data = data,
            };
        }
        protected Result<TResponse> Error(HttpStatusCode status, string key, object value)
        {
            return new Result<TResponse>(status, key, value);
        }
        protected Result<TResponse> Error(Result result)
        {
            return new Result<TResponse>(result.GetStatus()) { errors = result.errors };
        }

        private Result<TResponse> FromCache()
        {
            if (this.cache == null)
                return null;

            var result = MemoryCache.Default.Get(this.cache.Key) as Result<TResponse>;
            if (result != null)
                this.cache.found = true;

            return result;
        }
        private void SaveCache(Result<TResponse> result)
        {
            // cache not set
            // cache found data
            // result is null
            // result is not OK
            if (this.cache == null || this.cache.found || result == null || result.GetStatus() != HttpStatusCode.OK)
                return;

            MemoryCache.Default.Add(this.cache.Key, result, DateTimeOffset.Now.Add(this.cache.TimeSpan));
        }
        protected virtual Cache EnableCaching()
        {
            return null;
        }

        protected virtual Result<TResponse> Validation()
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
                                            validationResults.Select(vr => new KeyValuePair<string, object>(vr.MemberNames.First(), vr.ErrorMessage))
                                                .ToDictionary(vr => vr.Key, vr => vr.Value));

            return null;
        }
        protected virtual Result<TResponse> OnExecuting()
        {
            return null;
        }
        protected virtual Result<TResponse> Execute()
        {
            return null;
        }
        protected virtual async Task<Result<TResponse>> ExecuteAsync()
        {
            return null;
        }
        protected virtual Result<TResponse> OnSuccess(Result<TResponse> result)
        {
            return result;
        }
        protected virtual Result<TResponse> OnError(Result<TResponse> result)
        {
            return result;
        }
        protected virtual Result<TResponse> OnExecuted(Result<TResponse> result)
        {
            return result;
        }
        protected virtual Result<TResponse> UnhandledException(Exception ex)
        {
            return new Result<TResponse>(HttpStatusCode.InternalServerError);
        }


        public Result<TResponse> RunExecute(TRequest request)
        {
            this.request = request;
            Result<TResponse> result;
            this.cache = EnableCaching();

            try
            {
                result = Validation() ?? OnExecuting() ?? FromCache() ?? Execute() ?? ExecuteAsync().Result;
                SaveCache(result);

                // result is null
                result = result ?? new Result<TResponse>(HttpStatusCode.NoContent);

                // success
                if (result.IsSuccess)
                    result = OnSuccess(result);

                // error
                else
                    result = OnError(result);

                // finally
                result = OnExecuted(result);
            }
            // unhandled exception
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }
            return result;
        }
        public async Task<Result<TResponse>> RunExecuteAsync(TRequest request)
        {
            this.request = request;
            Result<TResponse> result;
            this.cache = EnableCaching();

            try
            {
                result = await Task.Run(async () => (Validation() ?? OnExecuting() ?? FromCache() ?? await ExecuteAsync() ?? Execute()));
                SaveCache(result);

                // result is null
                result = result ?? new Result<TResponse>(HttpStatusCode.NoContent);

                // success
                if (result.IsSuccess)
                    result = OnSuccess(result);

                // error
                else
                    result = OnError(result);

                // finally
                result = OnExecuted(result);
            }
            // unhandled exception
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }
            return result;
        }
    }
}
