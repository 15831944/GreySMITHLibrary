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
        private Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        private Database Database
        {
            get { return Document.Database; }
        }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<BlockTableRecord> BlockTableRecords
        {
            get
            {
                using (Transaction blockTrans = Database.TransactionManager.StartTransaction())
                {
                    //Open the database to read what blocks are in the drawing
                    IEnumerable<ObjectId> BlockIds =
                        (blockTrans.GetObject(Database.BlockTableId, OpenMode.ForRead) as BlockTable)
                            .Cast<ObjectId>();

//                    IEnumerable<BlockTableRecord> BlockTableRecordList = 
//                        BlockIds.SelectMany(
//                            blocktableID => (BlockTableRecord) blockTrans.GetObject(blocktableID, OpenMode.ForRead));
                }
            }
        }
        public List<AutoCADLayout> Layouts
        {
            get;
            set;
        }

        private void Initialize()
        {
        }
    }
}
