using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace RinneganCore
{
    public class Result
    {
        #region Properties
        internal int _statusCode => (int)_status;
        protected HttpStatusCode _status;
        public Dictionary<string, object?> errors { get; set; }
        public bool IsSuccess => (_statusCode >= 200 && _statusCode < 300);
        public HttpStatusCode status => _status;
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

        [JsonConstructorAttribute]
        public Result(HttpStatusCode status) : base(status)
        {
        }
        public Result(HttpStatusCode status, string errorKey, object? errorValue) : base(status, errorKey, errorValue)
        {
        }
        public Result(HttpStatusCode status, Dictionary<string, object?> errors) : base(status, errors)
        {
        }
        public Result(HttpStatusCode status, string json) : base(status)
        {
            Newtonsoft.Json.JsonConvert.PopulateObject(json, this);
        }
    }

    internal class ResultDto<T>
    {
        public ResultDto(Result<T> result)
        {
            this.data = result.data;
            this.errors = result.errors;
        }

        public T? data { get; set; }
        public Dictionary<string, object?>? errors { get; set; }
    }

}
