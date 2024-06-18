using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Runtime.CompilerServices;

namespace RinneganCore
{
    public class Result
    {
        #region Properties
        protected HttpStatusCode _status;
        public Dictionary<string, object?> errors { get; set; }
        #endregion

        #region Constractors
        private Result()
        {
            this.errors = new Dictionary<string, object?>();
        }
        public Result(HttpStatusCode status)
        {
            this._status = status;
            this.errors = new Dictionary<string, object?>();
        }
        public Result(HttpStatusCode status, string errorKey, object errorValue)
        {
            this._status = status;
            this.errors = new Dictionary<string, object?>();
            this.errors.Add(errorKey, errorValue);
        }
        internal Result(HttpStatusCode status, Dictionary<string, object?> errors)
        {
            this._status = status;
            this.errors = errors;
        }
        #endregion

        #region Functions
        public void AddError(string key, object value)
        {
            this.errors.Add(key, value);
        }
        public HttpStatusCode GetStatus()
        {
            return this._status;
        }
        #endregion
    }

    public class Result<T> : Result
    {
        public T? data { get; set; }

        public Result(HttpStatusCode status) : base(status)
        {
        }
        public Result(HttpStatusCode status, string errorKey, object? errorValue) : base(status, errorKey, errorValue)
        {
        }
        internal Result(HttpStatusCode status, Dictionary<string, object?> errors) : base(status, errors)
        {
        }

        public ObjectResult AsObjectResult()
        {
            return new ObjectResult(this)
            {
                StatusCode = (int)this._status,
            };
        }
    }


    //public enum ResultStatus
    //{
    //    OK = 200,                       //  success
    //    Redirect = 302,                 //  domain is not default
    //    Unauthorized = 401,             //  not authenticated
    //    Forbidden = 403,                //  not authorized
    //    NotFound = 404,                 //  domain not found
    //    BadRequest = 400,               //  input error
    //    UnprocessableContent = 422,     //  unable to execute by condition
    //    UnSuccess = 500,                //  internal server error
    //    NotImplemented = 501,           //  not implemented
    //    MissingPayload = 512,           //  missing payload
    //}
}
