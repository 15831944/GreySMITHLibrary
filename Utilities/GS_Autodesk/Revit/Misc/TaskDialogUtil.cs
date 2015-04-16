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

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Misc
{
    public static class TaskDialogUtil
    {
        public static TaskDialog Create(
            string windowtitle,
            string maintext,
            TaskDialogIcon mainicon)
        {
            TaskDialog td = new TaskDialog(windowtitle);
            td.MainInstruction = maintext;
            td.MainIcon = mainicon;

            return td;
        }
    }
}
