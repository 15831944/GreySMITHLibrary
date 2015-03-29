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

namespace GreySMITH.Utilities.Autodesk.Revit.Extensions.Documents
{
    public static class FamilyUtil
    {
        /// <summary>
        /// Method designed to directly load a family into a project using only a family symbol
        /// </summary>
        /// <param name="curdoc"></param>
        /// <param name="famsym"></param>
        /// <param name="cmd">External Command Data from the current command</param>
        /// <param name="doctoloadfrom">Document the family symbol should be loaded from</param>
        public static FamilySymbol LoadFamilyDirect(this Document curdoc, FamilySymbol famsym, Document doctoloadfrom, ExternalCommandData cmd)
        {
            // grab the application and change the active document
            UIApplication uiapp = cmd.Application;
            OpenOptions oop = new OpenOptions();

            oop.AllowOpeningLocalByWrongUser = true;
            oop.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;

            // make "doctoloadfrom" the active document
            if (uiapp.ActiveUIDocument.Document != doctoloadfrom)
            {
                try
                {
                    Document newcurdoc = curdoc.Application.OpenDocumentFile(ModelPathUtils.ConvertUserVisiblePathToModelPath(doctoloadfrom.PathName), oop);
                }

                catch
                {
                    uiapp.OpenAndActivateDocument(ModelPathUtils.ConvertUserVisiblePathToModelPath(doctoloadfrom.PathName), oop, false);
                    throw new Exception("Failed in attempt to open file. File name is:" + doctoloadfrom.PathName);
                }
            }

            Family newfamily = famsym.Family;
            Document doc_family = doctoloadfrom.EditFamily(newfamily);
            doc_family.LoadFamily(curdoc);
            doc_family.Close(false);
            //doctoloadfrom.Close(true);

            // return control to the original active document
            if (uiapp.ActiveUIDocument.Document != curdoc)
            {
                while (uiapp.ActiveUIDocument.Document != curdoc)
                {
                    uiapp.ActiveUIDocument.Document.Close(false);
                }
            }

            using (Transaction tr_regen = new Transaction(curdoc, "Regenerating the document..."))
            {
                tr_regen.Start();
                // regenerate that document
                curdoc.Regenerate();
                tr_regen.Commit();
            }

            return famsym;
        }

        public static void LoadFamilyDirect(this Document curdoc, FilteredElementCollector fec, ExternalCommandData excmd)
        {
            var collectionoffamsyms = from element in fec
                                        where element is FamilySymbol
                                        select element;

            foreach (FamilySymbol fs in collectionoffamsyms)
            {
                FamilySymbol curfamysym = curdoc.LoadFamilyDirect(fs, fs.Document, excmd);
            }
        }
        /// <summary>
        /// Returns truth value on whether the document contains the family symbol in question
        /// </summary>
        /// <param name="doc">Document to check</param>
        /// <param name="famsym">Family symbol to check for</param>
        /// <returns></returns>
        public static bool HasFamily(this Document doc, FamilySymbol famsym)
        {
            bool answer = false;
            List<Element> listoffams = new List<Element>();

            listoffams = doc.GetAllElements(famsym);
            var fammatches = from fam in listoffams
                             where fam.Name.Equals(famsym.Name)
                             select fam;

            if (fammatches.Count() > 0)
                answer = true;

            return answer;
        }

    }
}
