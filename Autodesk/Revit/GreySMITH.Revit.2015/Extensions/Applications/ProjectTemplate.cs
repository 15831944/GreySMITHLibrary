using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GreySMITH.Common.Utilities.General;

namespace GreySMITH.Revit.Extensions.Applications
{
    public static partial class ProjectTemplate
    {
        /// <summary>
        /// Shows the user a dialog box which displays choices for possible Revit Templates
        /// </summary>
        /// <returns>User's choice of template</returns>
        private static TaskDialogResult Choose()
        {
            TaskDialog RVT = new TaskDialog("Revit Model Setup");
            RVT.MainInstruction = "Which discipline model would you like to create?";
            RVT.MainContent = "To begin, please choose which model you'd like to create from the choices below";

            // All the links for the dialog box for discipline models
            RVT.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Electrical Template");
            RVT.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Mechanical Template");
            RVT.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Plumbing|Fire Protection Template");
            RVT.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "Combined Discipline Template");

            // Set default and common behavior buttons
            RVT.CommonButtons = TaskDialogCommonButtons.Cancel;
            RVT.DefaultButton = TaskDialogResult.Cancel;

            TaskDialogResult tdr = RVT.Show();
            return tdr;
        }

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
                    uidoc = uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Electrical.GetStringValue()),
                        oops,
                        false);
                    break;
                case (TaskDialogResult.CommandLink2):
                    uidoc = uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Mechanical.GetStringValue()),
                        oops,
                        false);
                    break;
                case (TaskDialogResult.CommandLink3):
                    uidoc = uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.PlumbingFireProtection.GetStringValue()),
                        oops,
                        false);
                    break;
                case (TaskDialogResult.CommandLink4):
                    uidoc = uiApp.OpenAndActivateDocument(
                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.All.GetStringValue()),
                        oops,
                        false);
                    break;
            }
            #endregion

            return uidoc.Document;
        }
    }
}
