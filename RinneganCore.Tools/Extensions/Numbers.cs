using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools.Extensions
{
    public static class Numbers
    {
        private static readonly string[] pn = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        private static readonly string[] en = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };


        /// <summary>
        /// Cast English number format to Persian number format
        /// </summary>
        /// <returns>persian number format</returns>
        public static string ToPersianNumber(this string strNum)
        {
            string chash = strNum.Trim();
            for (int i = 0; i < 10; i++)
                chash = chash.Replace(en[i], pn[i]);
            return chash;
        }


        /// <summary>
        /// Cast Persian number format to English number format
        /// </summary>
        /// <returns>english number format</returns>
        public static string ToEnNumber(this string strNum)
        {
            string chash = strNum.Trim();
            for (int i = 0; i < 10; i++)
                chash = chash.Replace(pn[i], en[i]);
            return chash;
        }
    }
}
