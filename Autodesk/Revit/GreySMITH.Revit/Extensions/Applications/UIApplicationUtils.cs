using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GreySMITH.Revit.Commands.Extensions.Applications
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
