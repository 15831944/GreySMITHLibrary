using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class AutoCADUtilities
    {
        /// <summary>
        /// Returns a list of the BlockTableRecords in the current Document's BlockTable using their ObjectIDs
        /// </summary>
        /// <returns> A List of all the block table records in the current project </returns>
        public static List<BlockTableRecord> GetAllBtrs()
        {
            Document doc = global::Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Open the database to read what blocks are in the drawing
                BlockTable blt;
                blt =
                    trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                IEnumerable<ObjectId> id_list = blt.Cast<ObjectId>();
                List<BlockTableRecord> btr_list = new List<BlockTableRecord>();
                foreach (ObjectId obj in id_list)
                {
                    btr_list.Add(trans.GetObject(obj, OpenMode.ForRead) as BlockTableRecord);
                }
                return btr_list;

            }
        }
