using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rinnegan
{
    public class Rinnegan<TResponse, TRequest> : IRequest<TRequest>
    {
        public TRequest request { get; set; }

        public Rinnegan(TRequest _request)
        {
            this.request = _request;
        }




        protected Result<TResponse> OK(TResponse data)
        {
            return new Result<TResponse>(ResultStatus.OK)
            {
                data = data,
            };
        }
        protected Result<TResponse> Error(ResultStatus status, string key, object value)
        {
            return new Result<TResponse>(status, key, value);
        }
        protected Result<TResponse> Error(Result result)
        {
            return new Result<TResponse>(result.GetStatus()) { errors = result.errors };
        }

        private Result<TResponse> Validation()
        {
            if (this.request == null && typeof(TRequest) != typeof(object))
                return new Result<TResponse>(ResultStatus.MissingPayload);
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
                return new Result<TResponse>(ResultStatus.BadRequest,
                                            validationResults.Select(vr => new KeyValuePair<string, object>(vr.MemberNames.First(), vr.ErrorMessage))
                                                .ToDictionary(vr => vr.Key, vr => vr.Value));

            return null;
        }

        protected virtual Result<TResponse> OnExecuting(Result<TResponse> result)
        {
            return result;
        }
        protected virtual Result<TResponse> Execute()
        {
            return new Result<TResponse>(ResultStatus.NotImplemented);
        }
        protected virtual Result<TResponse> OnExecuted(Result<TResponse> result)
        {
            return result;
        }

        protected virtual Result<TResponse> UnhandledException(Exception ex)
        {
            return new Result<TResponse>(ResultStatus.UnSuccess);
        }
        public Result<TResponse> RunExecute()
        {
            Result<TResponse> result = Validation();

            result = OnExecuting(result);

            if (result != null)
                return result;

            try
            {
                result = this.Execute();

                result = OnExecuted(result);

                return result;
            }
            catch (Exception ex)
            {
                return UnhandledException(ex);
            }
        }
        public async Task<Result<TResponse>> RunExecuteAsync()
        {
            Result<TResponse> result = Validation();

            result = OnExecuting(result);

            if (result != null)
                return result;

            try
            {
                result = await Task.Run<Result<TResponse>>(() => this.Execute());

                result = OnExecuted(result);

                return result;
            }
            catch (Exception ex)
            {
                return UnhandledException(ex);
            }
        }
    }
}
