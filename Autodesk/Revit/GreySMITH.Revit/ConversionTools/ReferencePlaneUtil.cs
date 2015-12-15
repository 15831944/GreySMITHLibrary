using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.ConversionTools
{
    public static class ReferencePlaneUtil
    {
        /// <summary>
        /// Creates a reference plane from a host object in the current view
        /// </summary>
        /// <param name="hostobject">Any object to which items can be hosted</param>
        /// <param name="curdoc">The current document</param>
        /// <returns>A reference plane which is the same length as the object itself</returns>
        public static ReferencePlane ConvertfromHostObject(this HostObject hostobject, Document curdoc)
        {
            ReferencePlane rp = null;

            // gets the length of the object 
            LocationCurve lcx = hostobject.Location as LocationCurve;
            XYZ startpoint = lcx.Curve.GetEndPoint(0);
            XYZ endpoint = lcx.Curve.GetEndPoint(1);
            XYZ cutvector = XYZ.BasisZ;

            // creates the reference plane in the current doc
            using (Transaction tr_referenceplane = new Transaction(curdoc, "Creating reference plane based on object..."))
            {
                tr_referenceplane.Start();
                rp = curdoc.Create.NewReferencePlane(startpoint, endpoint, cutvector, curdoc.ActiveView);
                rp.Name = "Reference to " + hostobject.Name;
                tr_referenceplane.Commit();
            }

            return rp;
        }

        public static void OtherMethod()
        {

        }

        public static void ThirdMethod()
        {

        }
    }
}
