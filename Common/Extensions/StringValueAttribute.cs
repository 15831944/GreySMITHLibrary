using System;

namespace GreySMITH.Common.Extensions
{
    /// <summary>
    /// An attribute to be used to create flags with enums
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        // private field which holds the value
        private string _value;

        // constructor
        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
