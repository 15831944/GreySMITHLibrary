using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADDrawing : AutoCADObject
    {
        public bool IsExternalReference;
        public bool HasExternalReferencesInPaperSpace;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IEnumerable<BlockTableRecord> BlockTableRecords
        {
            get { return AutoCADUtilities.RetrieveAllBlockTableRecords(Document); }
        }
        public List<AutoCADLayout> Layouts
        {
            get;
            set;
        }
        public IEnumerable<BlockTableRecord> ExternalReferences
        {
            get { return AutoCADUtilities.RetrieveExternalReferences(Document); }
        }
        private void Initialize(Document internalDocument)
        {
            Document = internalDocument;
        }
        public AutoCADDrawing(Document internalDocument)
        {
            Initialize(internalDocument);
        }
    }
}
