using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GreySMITH.Revit.Wrappers;

namespace GreySMITH.Revit.Extensions.Elements
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
