using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RinneganCore
{
    public class Rinnegan<TRequest, TResponse> : IRequest<TRequest>
    {
        public TRequest? request { get; set; }

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

        protected virtual Result<TResponse>? Validation()
        {
            if (this.request == null && typeof(TRequest) != typeof(object))
                return new Result<TResponse>(HttpStatusCode.BadRequest, "", "missing payload");
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
        protected virtual Result<TResponse>? OnExecuting()
        {
            return null;
        }
        protected virtual Result<TResponse> Execute()
        {
            return new Result<TResponse>(HttpStatusCode.NotImplemented);
        }
        protected virtual Result<TResponse> OnExecuted(Result<TResponse> result)
        {
            return result;
        }
        protected virtual Result<TResponse> UnhandledException(Exception ex)
        {
            return new Result<TResponse>(HttpStatusCode.InternalServerError);
        }


        public Result<TResponse> RunExecute(TRequest? request)
        {
            this.request = request;
            Result<TResponse> result;

            try
            {
                result = Validation() ?? OnExecuting() ?? Execute();
                result = OnExecuted(result);
            }
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }

            return result ?? new Result<TResponse>(HttpStatusCode.NoContent);
        }
        public async Task<Result<TResponse>> RunExecuteAsync(TRequest? request)
        {
            this.request = request;
            Result<TResponse> result;

            try
            {
                result = await Task.Run(() => (Validation() ?? OnExecuting() ?? Execute()));
                result = OnExecuted(result);
            }
            catch (Exception ex)
            {
                result = UnhandledException(ex);
            }

            return result ?? new Result<TResponse>(HttpStatusCode.NoContent);
        }
    }
}
