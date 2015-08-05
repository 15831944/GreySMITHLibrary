using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Documents
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
