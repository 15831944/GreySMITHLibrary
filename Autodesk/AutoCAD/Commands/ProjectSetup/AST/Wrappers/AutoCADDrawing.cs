using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADDrawing
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Document Document
        {
            get; set;
        }
        private Database Database
        {
            get { return Document.Database; }
        }
        private IEnumerable<BlockTableRecord> BlockTableRecords
        {
            get { return AutoCADUtilities.RetrieveAllBlockTableRecords(Document); }
        }
        public List<AutoCADLayout> Layouts
        {
            get;
            set;
        }
        private void Initialize()
        {
        }
        public AutoCADDrawing(Document internalDocument)
        {
            Document = internalDocument;
            Initialize();
        }
    }
}
