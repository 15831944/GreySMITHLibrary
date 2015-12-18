using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD
{
    public static class AutoCADUtilities
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Returns a list of the BlockTableRecords in the current Document's BlockTable using their ObjectIDs
        /// </summary>
        /// <returns> A List of all the block table records in the current project </returns>
        public static IEnumerable<BlockTableRecord> RetrieveAllBlockTableRecords(Document documentToGetBlocksFrom)
        {
            Logger.Debug("Attempting to retrieve BlockTableRecords from {0}", documentToGetBlocksFrom.Name);
            IEnumerable<BlockTableRecord> blockTableRecords;

            using (Transaction blockTrans = documentToGetBlocksFrom.Database.TransactionManager.StartTransaction())
            {
                // Open the database to read what blocks are in the drawing
                // Cast the DBObjects into ObjectIDs
                // Use those ObjectIDs to grab all BlockTableRecords
                blockTableRecords =
                    (blockTrans.GetObject(documentToGetBlocksFrom.Database.BlockTableId, OpenMode.ForRead) as BlockTable)
                        .Cast<ObjectId>()
                        .Select(blocktableID => (BlockTableRecord) blockTrans.GetObject(blocktableID, OpenMode.ForRead));
            }

            return blockTableRecords;
        }
    }
}
