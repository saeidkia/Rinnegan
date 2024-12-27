using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;

namespace RinneganCore.Tools.Extensions
{
    public static class Strings
    {
        private static readonly string[] pn = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        private static readonly string[] en = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };


        /// <summary>
        /// Cast English number format to Persian number format
        /// </summary>
        /// <returns>persian number format</returns>
        public static string ToPersianNumber(this string @this)
        {
            string chash = @this.Trim();
            for (int i = 0; i < 10; i++)
                chash = chash.Replace(en[i], pn[i]);
            return chash;
        }


        /// <summary>
        /// Cast Persian number format to English number format
        /// </summary>
        /// <returns>english number format</returns>
        public static string ToEnglishNumber(this string @this)
        {
            string chash = @this.Trim();
            for (int i = 0; i < 10; i++)
                chash = chash.Replace(pn[i], en[i]);
            return chash;
        }


        /// <summary>
        /// Trim and replace persian numbers to english numbers
        /// it is safe to null inputs
        /// </summary>
        /// <param name="this">string that need to cleaned up</param>
        /// <returns>cleaned up result</returns>
        public static string Clean(this string @this)
        {
            if (string.IsNullOrEmpty(@this)) return @this;

            return @this.Trim().ToEnglishNumber();
        }


        /// <summary>
        /// return content type of given file path
        /// </summary>
        /// <param name="this">file path</param>
        /// <returns>content type</returns>
        public static string ContentType(this string @this)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(@this, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}
