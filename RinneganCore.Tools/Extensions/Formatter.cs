using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools.Extensions
{
    public static class Formatter
    {
        /// <summary>
        /// Cast given object to Json format using Newtonsoft
        /// </summary>
        /// <returns>json string</returns>
        public static string ToJson(this object @this)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(@this);
        }


        /// <summary>
        /// Cast string to given format json type using Newtonsoft
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T? FromJson<T>(this string @this)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(@this);
        }


        /// <summary>
        /// Cast string to given Enum format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Enum format object</returns>
        public static T ToEnum<T>(this string @this)
        {
            return (T)Enum.Parse(typeof(T), @this, true);
        }


        /// <summary>
        /// Cast Dictionary to given class type
        /// </summary>
        /// <typeparam name="TResult">response type</typeparam>
        /// <param name="this">Dictionary<string, object></param>
        /// <returns>TResult type object</returns>
        public static TResult? To<TResult>(this Dictionary<string, object> @this)
            where TResult : class
        {
            return @this.ToJson()
               .FromJson<TResult>();
        }


        /// <summary>
        /// Cast Dictianry to given class type
        /// </summary>
        /// <typeparam name="TResult">response type</typeparam>
        /// <typeparam name="TKey">dictionary key type</typeparam>
        /// <typeparam name="TValue">dictionary value type</typeparam>
        /// <param name="this">Dictionary<TKey,TValue></param>
        /// <returns>TResult type object</returns>
        public static TResult? To<TResult, TKey, TValue>(this Dictionary<TKey, TValue> @this)
            where TResult : class
        {
            return @this.ToJson()
               .FromJson<TResult>();
        }


        /// <summary>
        /// Cast Dictionary to given class type
        /// </summary>
        /// <typeparam name="TResult">response type</typeparam>
        /// <param name="this"></param>
        /// <returns>TResult type object</returns>
        public static TResult? To<TResult>(this IDictionary @this)
            where TResult : class
        {
            return @this.ToJson()
               .FromJson<TResult>();
        }
    }
}
