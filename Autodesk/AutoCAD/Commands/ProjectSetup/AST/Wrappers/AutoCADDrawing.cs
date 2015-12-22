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

        // you may face errors here because of a lack of checks to one whether
        // the values being added here already exist or are not null
        public List<AutoCADDrawing> ExternalReferences
        {
            get
            {
                
            }
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
