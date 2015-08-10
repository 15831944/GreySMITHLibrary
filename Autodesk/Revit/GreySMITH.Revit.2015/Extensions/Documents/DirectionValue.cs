using System;

namespace GreySMITH.Revit.Extensions.Documents
{
    public class DirectionValueAttribute : Attribute
    {
        // private field which holds the value
        private string _value;

        // constructor
        public DirectionValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
