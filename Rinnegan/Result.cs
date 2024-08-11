using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan
{
    public class Result
    {
        #region Properties
        private HttpStatusCode _status;

        public int status => (int)_status;
        public string message => _status.ToString();
        public Dictionary<string, object> errors { get; set; }
        public bool IsSuccess => (status >= 200 && status < 300);
        #endregion

        #region Constractors
        private Result()
        {
        }
        public Result(HttpStatusCode status)
        {
            this._status = status;
            this.errors = new Dictionary<string, object>();
        }
        public Result(HttpStatusCode status, string errorKey, object errorValue)
        {
            this._status = status;
            this.errors = new Dictionary<string, object>();
            this.errors.Add(errorKey, errorValue);
        }
        internal Result(HttpStatusCode status, Dictionary<string, object> errors)
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
        public T data { get; set; }

        public Result(HttpStatusCode status) : base(status)
        {
        }
        public Result(HttpStatusCode status, string errorKey, object errorValue) : base(status, errorKey, errorValue)
        {
        }
        public Result(HttpStatusCode status, Dictionary<string, object> errors) : base(status, errors)
        {
        }

        public RinneganJsonResult AsActionResult()
        {
            return new RinneganJsonResult(new ResultDto<T>(this));
        }
    }

    internal class ResultDto<T>
    {
        internal ResultDto(Result<T> result)
        {
            this.status = result.status;
            this.message = result.message;
            this.errors = result.errors;
            this.data = result.data;
        }

        public T data { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public Dictionary<string, object> errors { get; set; }
    }
}
