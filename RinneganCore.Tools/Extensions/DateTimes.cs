using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools.Extensions
{
    public static class DateTimes
    {
        /// <summary>
        /// Cast "1403/08/23 13:24:53" Jalali datetime format to DateTime object
        /// </summary>
        /// <returns>DateTime?</returns>
        public static DateTime? ToDateTime(this string @this)
        {
            if (string.IsNullOrEmpty(@this) || @this.Length != 19) return null;

            try
            {
                string[] parts = @this.Split(' ');
                if (parts.Length != 2)
                    return null;

                string[] timeParts = parts[1].Split(':');
                if (timeParts.Length != 3)
                    return null;

                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);

                string[] secondParts = timeParts[2].Split('.');
                int second = int.Parse(secondParts[0]);
                int milliSecond = secondParts.Length == 2 ? int.Parse(secondParts[1]) : 0;

                return parts[0].ToDateWithTime(hour, minute, second, milliSecond);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Cast "1403/08/23" Jalali date format to DateOnly object
        /// </summary>
        /// <returns>DateOnly?</returns>
        public static DateOnly? ToDateOnly(this string @this)
        {
            if (string.IsNullOrEmpty(@this) || @this.Length != 10) return null;

            try
            {
                string[] parts = @this.Split('/');

                if (parts.Length != 3)
                {
                    parts = @this.Split('-');
                }

                if (parts.Length != 3)
                {
                    return null;
                }
                PersianCalendar pc = new PersianCalendar();
                return DateOnly.FromDateTime(pc.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), 1, 1, 1, 1));
            }
            catch (Exception)
            {

                return null;
            }
        }


        /// <summary>
        /// Cast "1403/08/23" Jalali date format to DateTime object with parameters for time
        /// </summary>
        /// <param name="hours">value for hour with default value 0</param>
        /// <param name="min">value for minute wuth default value 0</param>
        /// <param name="sec">value for second with default value 0</param>
        /// <param name="mil">value for millisecond with default value 0</param>
        /// <returns></returns>
        public static DateTime? ToDateWithTime(this string @this, int hours = 0, int min = 0, int sec = 0, int mil = 0)
        {
            if (string.IsNullOrEmpty(@this) || @this.Length != 10) return null;

            try
            {
                string[] parts = @this.Split('/');

                if (parts.Length != 3)
                {
                    parts = @this.Split('-');
                }

                if (parts.Length != 3)
                {
                    return null;
                }
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), hours, min, sec, mil);
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// Cast "1403/08/23" Jalali date format to DateTime object with parameters for time
        /// </summary>
        /// <param name="time">TimeOnly value</param>
        /// <returns>DateTime?</returns>
        public static DateTime? ToDateWithTime(this string @this, TimeOnly time)
        {
            if (string.IsNullOrEmpty(@this) || @this.Length != 10) return null;

            try
            {
                string[] parts = @this.Split('/');

                if (parts.Length != 3)
                {
                    parts = @this.Split('-');
                }

                if (parts.Length != 3)
                {
                    return null;
                }
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), time.Hour, time.Minute, time.Second, time.Millisecond);
            }
            catch (Exception)
            {

                return null;
            }
        }


        /// <summary>
        /// Cast DateTime to Jalali string format
        /// </summary>
        /// <returns>"1403/08/23"</returns>
        public static string ToJalaliDate(this DateTime @this)
        {
            PersianCalendar pc = new PersianCalendar();

            return $"{pc.GetYear(@this)}/{pc.GetMonth(@this).ToString("00")}/{pc.GetDayOfMonth(@this).ToString("00")}";
        }


        /// <summary>
        /// Cast DateOnly to Jalali string format
        /// </summary>
        /// <returns>"1403/08/23"</returns>
        public static string ToJalaliDate(this DateOnly @this)
        {
            return new DateTime(@this, new TimeOnly(0, 0)).ToJalaliDate();
        }


        /// <summary>
        /// Cast DateTime to 8 character Time format
        /// </summary>
        /// <returns>"13:24:53"</returns>
        public static string ToTimeString(this DateTime @this , bool includeMilisecond = false)
        {
            return $"{@this.Hour.ToString("00")}:{@this.Minute.ToString("00")}:{@this.Second.ToString("00")}{(includeMilisecond?$".{@this.Millisecond}":"")}";
        }


        /// <summary>
        /// Cast TimeOnly to 8 character Time format
        /// </summary>
        /// <returns>"13:24:53"</returns>
        public static string ToTimeString(this TimeOnly @this, bool includeMilisecond = false)
        {
            return $"{@this.Hour.ToString("00")}:{@this.Minute.ToString("00")}:{@this.Second.ToString("00")}";
        }
    }
}
