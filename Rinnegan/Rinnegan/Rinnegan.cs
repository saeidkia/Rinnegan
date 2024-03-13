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
    public class Rinnegan<TDbContext, TResponse, TRequest> : IDbContext<TDbContext>, IRequest<TRequest>, IDisposable where TDbContext : DbContext
    {
        public TRequest request { get; set; }
        public TDbContext db { get; set; }

        public Rinnegan(TRequest _request)
        {
            this.db = (TDbContext)Activator.CreateInstance(typeof(TDbContext));
            this.request = _request;
        }

        public Rinnegan(TRequest _request, TDbContext _db)
        {
            this.db = _db;
            this.request = _request;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
        }

        protected Result<TResponse> OK(TResponse data)
        {
            return new Result<TResponse>(ResultStatus.OK)
            {
                data = data,
            };
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
                return new Result<TResponse>(ResultStatus.UnSuccess);
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
                return new Result<TResponse>(ResultStatus.UnSuccess);
            }
        }



        private const string pn = "۰۱۲۳۴۵۶۷۸۹";
        private const string en = "0123456789";
        protected string Clean(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string result = input.Trim();
            for (int i = 0; i < 10; i++)
                result = result.Replace(pn[i], en[i]);
            return result;
        }
    }

    public class Rinnegan<TDbContext, TResponse> : Rinnegan<TDbContext, TResponse, object> where TDbContext : DbContext
    {
        public Rinnegan() : base(false)
        { }
        public Rinnegan(TDbContext _db) : base(false, _db)
        { }
    }
}
