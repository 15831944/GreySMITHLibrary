using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog.LayoutRenderers.Wrappers;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class AutoCADUtilities
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        private static Database Database
        {
            get { return Document.Database; }
        }

        private static Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        private static string ExternalReferenceFolder
        {
            get
            {
                return InitializeSetupDirectory(@"\_Setup Files\Xrefs");
            }
        }

        private static string SetupFolder
        {
            get
            {
                return InitializeSetupDirectory(@"\_Setup Files");
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Copies all external refernces to the default ExternalReference folder for 
        /// AutoCAD setups
        /// </summary>
        /// <param name="overwrite">option to overwrite file in directory - default is false</param>
        public static void CopyExternalReferenceToSetupDirectory(bool overwrite = false)
        {
            foreach (string externalReference in EnumerateExternalReferences(Database.Filename))
            {
                File.Copy(externalReference, ExternalReferenceFolder, overwrite);
            }
        }

        /// <summary>
        /// Exports a BlockTableRecord out to another location using a WBlock methodology
        /// </summary>
        /// <param name="blockTableRecord"> The External Reference to be exported; must be a Block Table Record </param>
        public static void ExportExternalReferenceToFile(BlockTableRecord blockTableRecord)
        {
            // if the file has already been exported
            // OR the ExternalReference can't be resolved - fail early
            string newXrefPath = ExternalReferenceFolder + blockTableRecord.Name.ToUpper() + ".dwg";
            if (File.Exists(newXrefPath) || !CanExternalReferenceBeResolved(blockTableRecord))
                return;

            using (Transaction transaction = Database.TransactionManager.StartTransaction())
            {
                using (Database xrefDatabase = blockTableRecord.GetXrefDatabase(true))
                {
                    Database.WblockCloneObjects(new ObjectIdCollection(), blockTableRecord.ObjectId, new IdMapping(), DuplicateRecordCloning.Ignore, false);
                    transaction.Commit();

                    // save the Xref to the new directory
                    xrefDatabase.SaveAs(newXrefPath, DwgVersion.AC1021);
                }
            }
        }

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
                        .Select(blocktableID => (BlockTableRecord)blockTrans.GetObject(blocktableID, OpenMode.ForRead));
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

        public static IEnumerable<BlockTableRecord> RetrieveExternalReferences(Layout layout)
        {
            // examine the layout's blocks

            // establish if any are external references

            //return their records
                    
        }

        public static void DeleteEmptyLayouts(Document document)
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

        /// <summary>
        /// Returns a queryable collection of External References in the directory
        /// </summary>
        /// <param name="pathToStartSearch">place where the search shoudl start</param>
        /// <returns></returns>
        private static IEnumerable<string> EnumerateExternalReferences(string pathToStartSearch)
        {
            return Directory.EnumerateFiles(
                    Path.GetDirectoryName(pathToStartSearch),
                    "*.dwg",
                    SearchOption.AllDirectories);
        }

        private static string InitializeSetupDirectory(string directoryToInitialize)
        {
            // Get the current directory by finding the file path
            string docFilePath = Path.GetDirectoryName(Document.Database.Filename);

            // if the directory already exists, fail early
            if (Directory.Exists(docFilePath + directoryToInitialize))
                return (docFilePath + directoryToInitialize);

            //Creates a new directory for all files to be saved to
            //called "_Setup Files" and "Xrefs"
            return Directory.CreateDirectory(docFilePath + directoryToInitialize).ToString();
        }

        private static IEnumerable<Layout> RetrieveAllLayouts(Document document)
        {
            using (Transaction transaction = document.Database.TransactionManager.StartTransaction())
            {
                return (from BlockTableRecord blockTR in RetrieveAllBlockTableRecords(document)
                        where blockTR.IsLayout
                        select blockTR)
                        .Select(btr => (Layout) transaction.GetObject(btr.Id, OpenMode.ForRead));
            }
        }

        #endregion Methods
    }
}