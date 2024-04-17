using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
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
        private bool IsValid(out Result<TResponse> result)
        {
            result = null;
            if (this.request == null)
            {
                result = new Result<TResponse>(ResultStatus.BadRequest, nameof(this.request), "payload not found");
                return false;
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
                result = new Result<TResponse>(ResultStatus.BadRequest,
                                            validationResults.Select(vr => new KeyValuePair<string, object>(vr.MemberNames.First(), vr.ErrorMessage))
                                                .ToDictionary(vr => vr.Key, vr => vr.Value));

            return isValid;
        }
        protected virtual Result<TResponse> Execute()
        {
            return new Result<TResponse>(ResultStatus.NotImplemented);
        }
        protected virtual Result<TResponse> UnhandledException(Exception ex)
        {
            return new Result<TResponse>(ResultStatus.UnSuccess);
        }
        public Result<TResponse> RunExecute()
        {
            Result<TResponse> result;
            if (!IsValid(out result)) return result;

            try
            {
                return this.Execute();
            }
            catch (Exception ex)
            {
                return UnhandledException(ex);
            }
        }
        public async Task<Result<TResponse>> RunExecuteAsync()
        {
            Result<TResponse> result;
            if (!IsValid(out result)) return result;

            try
            {
                return await Task.Run<Result<TResponse>>(() => this.Execute());
            }
            catch (Exception ex)
            {
                return UnhandledException(ex);
            }
        }
    }
}
