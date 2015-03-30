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

namespace GreySMITH.Utilities.Autodesk.Revit.Extensions.Applications
{
    public static class UIApplicationUtils
    {
        public static void CloseAllButThis(this UIApplication uiapp, Document documentwhichshouldremain)
        {

        }

        public static Document SwitchActiveDocument(this UIApplication uiapp, Document documenttoswitchto)
        {
            return documenttoswitchto;
        }
    }
}
