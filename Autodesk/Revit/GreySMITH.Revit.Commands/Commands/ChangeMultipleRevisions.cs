using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Reflection;
using Autodesk.Revit.Attributes;
using NLog;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GreySMITH.Revit.Commands.Wrappers;
using NLog.LayoutRenderers.Wrappers;

namespace GreySMITH.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ChangeMultipleRevisions
    {
        public Result Execute(
            ExternalCommandData externalCommandData,
            ref string mainmessage,
            ElementSet elementSet)
        {
            // get the main Document
            Document currentDocument = externalCommandData.Application.ActiveUIDocument.Document;

            // collect the sheets in the model
            var AllSheetsInModel =
                from Sheet in (new FilteredElementCollector(currentDocument)
                    .OfCategory(BuiltInCategory.OST_Sheets)
                    .ToElements())
                select (ViewSheet) Sheet;
            // collect all the revisions in the model
//            var AllRevisionsInModel =
//                from Revision in (new FilteredElementCollector(currentDocument)
//                    .OfCategory(BuiltInCategory.OST_Revisions)
//                    .ToElements())
//                select Revision;

            List<Revision> ProjectRevisionList = new List<Revision>();

            IEnumerable<Revision> AllRevisionsInModel =
                new FilteredElementCollector(currentDocument)
                    .OfCategory(BuiltInCategory.OST_Revisions)
                    .ToElements()
                    .SelectMany(revision => ProjectRevisionList.Add(new Revision(
                        revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).ToString(),
                        revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).ToString(),
                        revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).ToString())));

            
            

            // examine the revisions that exist on each sheet
            foreach (ViewSheet sheet in AllSheetsInModel)
            {
                
            }

            return Result.Failed;
        }
    }

    public class Revision
    {
        public string SequenceId { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public Revision(string name, string number, string date)
        {
            Description = name;
            SequenceId = number;
            Date = date;
        }
    }
}
