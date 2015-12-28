using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using NLog;
using NLog.LayoutRenderers.Wrappers;
using Exception = System.Exception;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class AutoCADUtilities
    {
        private static Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }
        private static Database Database
        {
            get { return Document.Database; }
        }
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
        public static IEnumerable<BlockTableRecord> RetrieveExternalReferences(Document documentToGetReferencesFrom)
        {
            IEnumerable<BlockTableRecord> externalReferences;

            using (Transaction transaction = documentToGetReferencesFrom.TransactionManager.StartTransaction())
            {
                //Create a list of all xrefs in the file
                //Check the drawing to see if there are any xrefs
                externalReferences =
                    (from BlockTableRecord xrefBlock in
                        (RetrieveAllBlockTableRecords(documentToGetReferencesFrom))
                        where xrefBlock.IsFromExternalReference
                        select xrefBlock);

                //Complete the command
                transaction.Commit();
            }

            return externalReferences;
        }
        /// <summary> 
        /// Exports a BlockTableRecord out to another location using a WBlock methodology 
        /// </summary>
        /// <param name="blockTableRecord"> The External Reference to be exported; must be a Block Table Record </param>
        public static void ExportExternalReferenceToFile(BlockTableRecord blockTableRecord)
        {
            // Get the current directory by finding the file path
            string docFilePath = Path.GetDirectoryName(Document.Database.Filename);

            //Creates a new directory for all files to be saved to
            //called "_Setup Files" and "Xrefs"
            string setupDirectory = Directory.CreateDirectory(docFilePath + @"\_Setup Files\SETUP\Xrefs").ToString();

            // if the file has already been exported 
            // OR the ExternalReference can't be resolved - fail early
            string newXrefPath = setupDirectory + blockTableRecord.Name.ToUpper() + ".dwg";
            if (File.Exists(newXrefPath) || !CanExternalReferenceBeResolved(blockTableRecord))
                return;

            using (Transaction transaction = Database.TransactionManager.StartTransaction())
            {
                using (Database xrefDatabase = blockTableRecord.GetXrefDatabase(true))
                {
                    ObjectIdCollection idCollection = new ObjectIdCollection();
//                    IdMapping idMapping = new IdMapping();
                    Database.WblockCloneObjects(idCollection, blockTableRecord.ObjectId, new IdMapping(), DuplicateRecordCloning.Ignore, false);
                    transaction.Commit();
                    if (!Directory.Exists(newXrefPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newXrefPath));
                    }

                    xrefDatabase.SaveAs(newXrefPath, DwgVersion.AC1021);
                }
            }
        }
        ///<summary>
        /// Copies an External Reference from an existing location to another folder
        /// will overwrite any file with the same name
        ///</summary>
        public static void CopyExternalReferenceToDirectory(BlockTableRecord blockTableRecord)
        {
            
        }
        private static bool CanExternalReferenceBeResolved(BlockTableRecord blockTableRecord)
        {
            // if the External Reference can't be found nearby, just fail out
            while (!blockTableRecord.IsResolved)
            {
                int counter = 0;
                Database.ResolveXrefs(true, false);
                counter++;
                if (counter > 1)
                    return false;
            }

            return true;
        }

    }
}
