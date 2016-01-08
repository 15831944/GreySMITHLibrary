using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GreySMITH.Autodesk.AutoCAD.Wrappers;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class BlockTableRecordRetriever : IRetriever<Document, IEnumerable<BlockTableRecord>>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<BlockTableRecord> Retrieve(Document document)
        {
            Logger.Debug("Attempting to retrieve BlockTableRecords from {0}", document.Name);
            IEnumerable<BlockTableRecord> blockTableRecords;

            using (Transaction blockTrans = document.Database.TransactionManager.StartTransaction())
            {
                // Open the database to read what blocks are in the drawing
                // Cast the DBObjects into ObjectIDs
                // Use those ObjectIDs to grab all BlockTableRecords
                blockTableRecords =
                    (blockTrans.GetObject(document.Database.BlockTableId, OpenMode.ForRead) as BlockTable)
                        .Cast<ObjectId>()
                        .Select(blocktableID => (BlockTableRecord)blockTrans.GetObject(blocktableID, OpenMode.ForRead));
            }

            return blockTableRecords;
        }
    }
}
