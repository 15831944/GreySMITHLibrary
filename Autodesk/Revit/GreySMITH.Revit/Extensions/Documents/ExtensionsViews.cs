using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GreySMITH.Revit.Wrappers;

namespace GreySMITH.Revit.Extensions.Documents
{
    public static partial class ExtensionsViews
    {
        public static View3D Create3DView(this Document currentDocument)
        {
            // Filters the model for 3D Views
            FilteredElementCollector fec3DViews =
                new FilteredElementCollector(currentDocument);

            // creates an Isometric 3D View 
            return View3D.CreateIsometric(
                    currentDocument,
                    fec3DViews.OfType<View3D>().First().Id);
        }
    }
}
