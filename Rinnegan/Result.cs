using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public class Result<T>
    {
        #region Properties
        private ResultStatus _status;

        public int status => (int)_status;
        public string message => _status.ToString();
        public Dictionary<string, object> errors { get; set; }

        public T data { get; set; }
        #endregion


        #region Constractors
        private Result()
        {
        }
        public Result(ResultStatus status)
        {
            this._status = status;
            this.errors = new Dictionary<string, object>();
        }
        public Result(ResultStatus status, string errorKey, object errorValue)
        {
            this._status = status;
            this.errors = new Dictionary<string, object>();
            this.errors.Add(errorKey, errorValue);
        }
        internal Result(ResultStatus status, Dictionary<string, object> errors)
        {
            this._status = status;
            this.errors = errors;
        }
        #endregion

        #region Functions
        public void AddErrro(string key, object value)
        {
            this.errors.Add(key, value);
        }
        public ResultStatus GetStatus()
        {
            return this._status;
        }
        #endregion
    }


    public enum ResultStatus
    {
        OK = 200,                       //  success
        Redirect = 302,                 //  domain is not default
        Unauthorized = 401,             //  not authenticated
        Forbidden = 403,                //  not authorized
        NotFound = 404,                 //  domain not found
        BadRequest = 400,               //  input error
        UnprocessableContent = 422,     //  unable to execute by condition
        UnSuccess = 500,                //  internal server error
        NotImplemented = 501,           //  not implemented
    }
}
