using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;


namespace GreySMITH.Utilities.Autodesk.Revit.ConversionTools
{
    public static class ReferencePlaneUtil
    {
        public static void ConvertfromFace(Face objectface)
        {
            PlanarFace pof = objectface as PlanarFace;
            Wall w = new Wall();
            //LocationCurve lc = w.Location as LocationCurve;
            //XYZ startpoint = lc.Curve.get_EndPoint(0);
            //XYZ endpoint = lc.Curve.get_EndPoint(1);
            //XYZ height = lc
            

            
        }

        public static void OtherMethod()
        {

        }

        public static void ThirdMethod()
        {

        }
    }
}
