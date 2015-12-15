using Autodesk.Revit.UI;

namespace GreySMITH.Revit.Commands.Misc
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
