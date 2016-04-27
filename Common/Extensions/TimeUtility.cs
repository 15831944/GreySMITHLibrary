using System;

namespace GreySMITH.Common.Extensions
{
    /// <summary>
    /// Contains Custom utilities for dealing with time
    /// </summary>
    public static class TimeUtility
    {
        /// <summary>
        /// Returns a string with the DateTime value in the YYYY-MM-DD format
        /// </summary>
        /// <param name="datetime">Date Time to be formatted</param>
        /// <returns>YYYY-MM-DD_0000 if using time of day</returns>
        public static string DateFormatter( DateTime datetime, bool usetimeofday = false)
        {
            string finaldt;

            var year = datetime.Year.ToString();
            var month = datetime.Month.ToString();
            if (month.Length < 2)
            {
                month = "0" + month;
            }
            var Day = datetime.Day.ToString();
            if (Day.Length < 2)
            {
                Day = "0" + Day;
            }

            var hour = datetime.Hour.ToString();
            var minute = datetime.Minute.ToString();

            if (usetimeofday)
                finaldt = year + "-" + month + "-" + Day + "_" + hour + minute;

            else
                finaldt = year + "-" + month + "-" + Day;

            return finaldt;
        }
    }
}
