using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GreySMITH.Revit.Misc;

namespace GreySMITH.Revit.Extensions.Documents
{
    /// <summary>
    /// Class of items which deal with extension to Revit Linking capabilities
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    public static class RevitLinkUtil
    {
        #region InstanceMaker Methods
        /// <summary>
        /// Creates an instance for the Revit file selected
        /// </summary>
        /// <param name="doc">Document to which the link should be added</param>
        /// <param name="revitFilePath">the lists of path of the Revit links to be added</param>
        private static void InstanceMaker(Document doc, List<string> revitFilePath)
        {
            foreach (string rfp in revitFilePath)
            {
                RevitLinkUtil.InstanceMaker(doc, rfp);
            }
        }

        /// <summary>
        /// Creates an instance for the Revit file selected
        /// </summary>
        /// <param name="doc">Document to which the link should be added</param>
        /// <param name="revitFilePath">the full path of the Revit link to be added</param>
        private static void InstanceMaker(Document doc, string revitFilePath)
        {
            try
            {
                using (Transaction tr = new Transaction(doc))
                {
                    tr.Start("Revit files are being linked...");

                    // Cycle through the list
                    // Set the standard options and behavior for the links
                    FilePath fp = new FilePath(revitFilePath);
                    RevitLinkOptions rlo = new RevitLinkOptions(false);

                    // Create new revit link and store the path to the file as either absolute or relative
                    RevitLinkLoadResult result = RevitLinkType.Create(doc, fp, rlo);
                    ElementId linkId = result.ElementId;

                    // Create the Revit Link Instance (this also automatically sets the link Origin-to-Origin)
                    // Pin the Revit link as well
                    RevitLinkInstance linkInstance = RevitLinkInstance.Create(doc, linkId);
                    linkInstance.Pinned = true;

                    tr.Commit();
                }
            }

            catch (Exception)
            {
                // not sure what exceptions may occur
                // make sure that whatever happens is logged for future troubleshooting
            }

        }
        #endregion

        /// <summary>
        /// Links in a Revit File or files
        /// using a Dialog box within the active document; 
        /// pins the instance of the Revit Link Origin-to-Origin automatically.
        /// Returns the name of the main file which should be used for Copy-Monitoriing etc
        /// </summary>
        /// <param name="doc">Active Document</param>
        public static ElementId CreateLinkRevit(this Document doc)
        {
            // Ask the user which links they would like to add to the project
            // Call the ShowDialog method to show the dialog box filtered to show only Revit Projects
            // Choose the main link to base the document on for Copy-Monitoring
            TaskDialog td_mainrvtlnk = TaskDialogUtil.Create("Main Revit Link",
                                                                "Select the link which should be used to set up Grids and Levels for Copy Monitoring",
                                                                TaskDialogIcon.TaskDialogIconNone);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Revit Documents|*.rvt";

            TaskDialog td_addtlrvtlnk = TaskDialogUtil.Create("Additional Revit Links",
                                                                 "Select any additional links which should be added to the project",
                                                                 TaskDialogIcon.TaskDialogIconNone);
            OpenFileDialog ofd2 = new OpenFileDialog();
            ofd2.Multiselect = true;
            ofd2.Filter = "Revit Documents|*.rvt";
            td_mainrvtlnk.Show();

            DialogResult dr = ofd.ShowDialog();

            //If the user clicks ok - record the name of main RVT Link
            if (dr == System.Windows.Forms.DialogResult.OK || dr == DialogResult.Cancel)
            {
                //Add the link selected to a list for later
                string lnk_cm = ofd.FileName;

                td_addtlrvtlnk.Show();
                DialogResult dr2 = ofd2.ShowDialog();
                //If the user clicks ok - continue
                if (dr2 == System.Windows.Forms.DialogResult.OK || dr2 == DialogResult.Cancel)
                {
                    //Link in the revit files
                    //Add the links selected to a list
                    List<string> rvtfiles = new List<string>();
                    rvtfiles.Add(ofd.FileName);
                    foreach (string x in ofd2.FileNames)
                    {
                        rvtfiles.Add(x);
                    }
                    InstanceMaker(doc, rvtfiles);
                }
            }

            // Gets the elementID of the link you want for copy-monitoring
            ElementId cm_linkid = RevitLinkType.GetTopLevelLink(doc,
                ModelPathUtils.ConvertUserVisiblePathToModelPath(ofd.FileName));

            return cm_linkid;

        }
    }
}
