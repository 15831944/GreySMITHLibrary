using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.Extensions.Elements
{
    public static partial class ExtensionsConnectors
    {
        public static bool IntersectsWithOwnerElement(this Connector c, Element element)
        {
//            ReferenceWithContext rwc = new ReferenceIntersector(
//                element.Id,
//                FindReferenceTarget.All,
//                Create3DView()).FindNearest(c.Origin, c.CoordinateSystem.BasisZ);
//
//

            return false;
        }
    }
}
