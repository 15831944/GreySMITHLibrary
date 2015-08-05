using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Documents
{

    public enum XYZDirection
    {
        [DirectionValue("X")]
        X = 1,
        [DirectionValue("Y")]
        Y = 2,
        [DirectionValue("Z")]
        Z = 3,
    }

    public static class XYZDirectionEnum
    {
        public static string GetDirectionValue(this XYZDirection value)
        {
            string output = null;

            try
            {
                Type type = value.GetType();

                FieldInfo fi = type.GetField(value.ToString());
                DirectionValueAttribute[] attrs =
                    fi.GetCustomAttributes(typeof(DirectionValueAttribute),
                                            false) as DirectionValueAttribute[];

                output = attrs.FirstOrDefault().Value;
            }

            catch (ArgumentNullException ane)
            {
                Console.WriteLine("There are no values in this object" + ane.StackTrace.ToString());
            }


            return output;
        }
    }
}
