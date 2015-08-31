using System.Linq;
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
//        public static Document CreateFromTemplate(this UIApplication uiApp)
//        {
//            _uiApplication = uiApp;
//            // prompt user to pick the template
//            TaskDialogResult tdr = Choose();
//
//            SetDocumentOpenOptions();
//
//            UIDocument uidoc = null;
//
//            // Based on the selection in the task dialog, choose the appropiate template
//            #region Possible Disciplines (still need one for tech)
//            switch (tdr)
//            {
//                case (TaskDialogResult.CommandLink1):
//                    return uiApp.OpenAndActivateDocument(
//                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Electrical.GetStringValue()),
//                        _openOptions,
//                        false).Document;
//                case (TaskDialogResult.CommandLink2):
//                    return uiApp.OpenAndActivateDocument(
//                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.Mechanical.GetStringValue()),
//                        _openOptions,
//                        false).Document;
//                case (TaskDialogResult.CommandLink3):
//                    return uiApp.OpenAndActivateDocument(
//                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.PlumbingFireProtection.GetStringValue()),
//                        _openOptions,
//                        false).Document;
//                case (TaskDialogResult.CommandLink4):
//                    return uiApp.OpenAndActivateDocument(
//                        ModelPathUtils.ConvertUserVisiblePathToModelPath(TemplateDiscipline.All.GetStringValue()),
//                        _openOptions,
//                        false).Document;
//                default:
//                    return null;
//            }
//            #endregion
//        }

        static partial void SetDocumentOpenOptions()
        {
            #region DocumentOptions (look into making this a separate item)
            // set options for typical document opening
            WorksetConfiguration wrkcon = new WorksetConfiguration();
            wrkcon.Close(
                new FilteredWorksetCollector(
                    _uiApplication.ActiveUIDocument.Document)
                    .ToWorksetIds()
                    .ToList());


            Document doc = _uiApplication.ActiveUIDocument.Document;
            //var list_elems = from 
            //var worksetlist = doc.GetWorksetId()


            _openOptions.Audit = true;
            _openOptions.DetachFromCentralOption = DetachFromCentralOption.ClearTransmittedSaveAsNewCentral;
            _openOptions.SetOpenWorksetsConfiguration(wrkcon);
            #endregion
        }
    }
}
