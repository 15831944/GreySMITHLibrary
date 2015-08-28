using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GreySMITH.Common.Utilities.General;

namespace GreySMITH.Revit.Extensions.Applications
{
    public static partial class ProjectTemplate
    {


        /// <summary>
        /// Creates a project based a on a template chosen by the user
        /// </summary>
        /// <param name="uiApp">Current UIApplication</param>
        /// <returns>The document chosen by the user</returns>
        public static Document CreateFromTemplate(this UIApplication uiApp)
        {
            // prompt user to pick the template
            TaskDialogResult tdr = ProjectTemplate.Choose();

            #region DocumentOptions (look into making this a separate item)
            // set options for typical document opening
            WorksetConfiguration wrkcon = new WorksetConfiguration();
            wrkcon.OpenLastViewed();
            uiApp.ActiveUIDocument.Document
            

            Document doc = uiApp.ActiveUIDocument.Document;
            //var list_elems = from 
            //var worksetlist = doc.GetWorksetId()

            OpenOptions oops = new OpenOptions();
            oops.Audit = true;
            oops.DetachFromCentralOption = DetachFromCentralOption.ClearTransmittedSaveAsNewCentral;
            oops.SetOpenWorksetsConfiguration(wrkcon);
            #endregion

            UIDocument uidoc = null;

            // Based on the selection in the task dialog, choose the appropiate template
            #region Possible Disciplines (still need one for tech)
            switch (tdr)
            {
                case (TaskDialogResult.CommandLink1):
                    return uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Electrical.GetStringValue()),
                        oops,
                        false).Document;
                case (TaskDialogResult.CommandLink2):
                    return uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Mechanical.GetStringValue()),
                        oops,
                        false).Document;
                case (TaskDialogResult.CommandLink3):
                    return uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.PlumbingFireProtection.GetStringValue()),
                        oops,
                        false).Document;
                case (TaskDialogResult.CommandLink4):
                    return uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.All.GetStringValue()),
                        oops,
                        false).Document;
                default:
                    return null;
            }
            #endregion
        }
    }
}
