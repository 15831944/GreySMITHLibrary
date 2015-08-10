namespace GreySMITH.Common.Utilities.General
{
    static class StringUtils
    {
        /// <summary>
        /// Subtracts one string from a larger string
        /// </summary>
        /// <param name="x"> String to delete from </param>
        /// <param name="y"> String to be deleted </param>
        /// <returns> A string value </returns>
        public static string Subtract(this string x, string y)
        {
            //Takes two strings and subtracts the second value from the first
            string z = x.TrimEnd(y.ToCharArray());
            return z;
        }
    }
}
