using System.Collections.Generic;
using System.Linq;

namespace GreySMITH.Common.Extensions
{
    /// <summary>
    /// Class for extending functionality of miscellaneous system objects
    /// </summary>
    public static class DictionaryUtil
    {
        /// <summary>
        /// Takes two Enumerables and returns a dictionary with the values zipped together
        /// </summary>
        /// <param name="enumerA">Enumerable of keys</param>
        /// <param name="enumerB">Enumerable of values</param>
        /// <returns>Dictionary of the contents of the list</returns>
        public static Dictionary<object, object> Create(IEnumerable<object> enumerA, IEnumerable<object> enumerB)
        {
            var dict =
                enumerA.Zip(enumerB, (k, v) =>
                    new { k, v }).ToDictionary(
                    x => x.k,
                    x => x.v);

            return dict;
        }
    }
}
