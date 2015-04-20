using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreySMITH.Utilities.General.Time
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
        /// <returns>YYYY-MM-DD</returns>
        public static string DateFormatter( DateTime datetime)
        {
            string finaldt;

            string Year = datetime.Year.ToString();
            string Month = datetime.Month.ToString();
            if(Month.Length < 2)
            {
                Month = "0" + Month;
            }
            string Day = datetime.Day.ToString();
            if (Day.Length < 2)
            {
                Day = "0" + Day;
            }

            finaldt = Year + "-" + Month + "-" + Day;

            return finaldt;
        }

        public static string DateFormatter( DateTime datetime, bool usetimeofday)
        {
            string finaldt;

            string Year = datetime.Year.ToString();
            string Month = datetime.Month.ToString();
            if (Month.Length < 2)
            {
                Month = "0" + Month;
            }
            string Day = datetime.Day.ToString();
            if (Day.Length < 2)
            {
                Day = "0" + Day;
            }

            string Hour = datetime.Hour.ToString();
            string Minute = "00";

            finaldt = Year + "-" + Month + "-" + Day + "_" + Hour + Minute;

            return finaldt;
        }
    }
}
