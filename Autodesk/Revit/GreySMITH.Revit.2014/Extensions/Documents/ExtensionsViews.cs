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
        private static RevitExtension _revitExtension;

        private static Document CurrentDocument
        {
            get { return _revitExtension.CurrentDocument; }
        }

        /* TODO: FIX EXENSIONVIEWS AND REVITEXTENSION CLASS|INTERFACE
        this isn't complete at all - you still have to remember to set this
        so that it grabs the REAL CurrentDocument from which ever command is currently 
        using it
        */

        public static View3D Create3DView()
        {
            // Filters the model for 3D Views
            FilteredElementCollector fec3DViews =
                new FilteredElementCollector(CurrentDocument);

            // creates an Isometric 3D View 
            return View3D.CreateIsometric(
                    CurrentDocument,
                    fec3DViews.OfType<View3D>().First().Id);
        }
    }
}
