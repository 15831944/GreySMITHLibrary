using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ExtensionMethods;


namespace ExtensionMethods
{
    public static class Extensions_Document
    {
        public static bool HasEmptyLayouts(this Document doc)
        {
            bool value = false;

            Database db = doc.Database;
            DocumentLock lckdoc = doc.LockDocument();
            Editor ed = doc.Editor;

            //Gather all the xrefs from the document, if there are any
            List<BlockTableRecord> xlist = Project_Setup.ProjectSetup.Xref_List(doc);

            //Get a list of layouts/layoutids
            List<Layout> listoflayouts = new List<Layout>();
            List<ObjectId> listoflayids = Project_Setup.ProjectSetup.Project_ListOfLayoutIds(doc);
            ed.WriteMessage("\n There are {0} layouts in this model", listoflayids.Count);

            // The objectIDs for the BlockTableRecords of the Model and PaperSpace layouts
            var mspace = listoflayids[0];
            var pspaces = listoflayids.GetRange(1, (listoflayids.Count() - 1));

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (Layout l in listoflayouts)
                {
                    ObjectId lbtr_id = l.BlockTableRecordId;
                    BlockTableRecord l_btr = tr.GetObject(lbtr_id, OpenMode.ForRead) as BlockTableRecord;

                    List<ObjectId> objs_inLayout = new List<ObjectId>();
                    foreach (ObjectId x in l_btr)
                    {
                        objs_inLayout.Add(x);
                    }

                    int o = objs_inLayout.Count();

                    if (o <= 1)
                    {
                        value = true;
                        return value;
                    }
                }
            }
            return value;
        }
        public static bool HasXrefsInMSpace(this Document doc)
        {
            bool value = false;

            var btrlist = Project_Setup.ProjectSetup.GetAllBtrs();
            var xreflist = Project_Setup.ProjectSetup.Xref_List(doc);

            List<ObjectId> listoflayids = Project_Setup.ProjectSetup.Project_ListOfLayoutIds(doc);
            var mspace = listoflayids[0];

            if (xreflist.Count() > 0)
            {
                foreach (BlockTableRecord x in xreflist)
                {
                    ObjectId[] xref_layids = Project_Setup.ProjectSetup.Xref_LayoutFinder(x);
                    if (xref_layids.Any().Equals(mspace) || xref_layids.Any().Equals(mspace))
                    {
                        value = true;
                        return value;
                    }
                }
            }

            return value;
        }
        public static bool HasXrefsInPSpace(this Document doc)
        {
            bool value = false;

            var btrlist = Project_Setup.ProjectSetup.GetAllBtrs();
            var xreflist = Project_Setup.ProjectSetup.Xref_List(doc);

            List<ObjectId> listoflayids = Project_Setup.ProjectSetup.Project_ListOfLayoutIds(doc);
            var pspaces = listoflayids.GetRange(1, (listoflayids.Count() - 1));

            if (xreflist.Count() > 0)
            {
                foreach (BlockTableRecord x in xreflist)
                {
                    ObjectId[] xref_layids = Project_Setup.ProjectSetup.Xref_LayoutFinder(x);
                    if (xref_layids.Any().Equals(pspaces.Any()) || xref_layids.Any().Equals(pspaces.Any()))
                    {
                        value = true;
                        return value;
                    }
                }
            }

            return value;
        }
        /// <summary>
        /// Creates a new document while within another document based on a 
        /// template defined within the parameters
        /// </summary>
        /// <param name="doc">The name of the current document</param>
        /// <param name="template">The string which defines the template's full path and name</param>
        /// <param name="directorytosavein">The directory where the newly made document should be saved</param>
        /// <returns>A new document</returns>
        public static Document CreateNew(this Document doc, string template, string directorytosavein, string dwgname)
        {
            //create a new dwg based on the BR+A template
            DocumentCollection docmgr = Application.DocumentManager;
            Document newdoc = docmgr.Add(template);

            //switch the active drawing to the newly made drawing
            docmgr.MdiActiveDocument = newdoc;
            newdoc.Database.SaveAs(
                Path.Combine((directorytosavein), ("Setup_" + (Path.GetFileNameWithoutExtension(doc.Name) + "_") + (dwgname) + ".dwg")),
                true,
                DwgVersion.AC1800,
                doc.Database.SecurityParameters);

            return newdoc;
        }
        /// <summary>
        /// An extension method which adds an external reference to the current document
        /// </summary>
        /// <param name="doc">The document which should have the xref added to it</param>
        /// <param name="xref_fullpath">The path of the xref to be added</param>
        /// <param name="xref_name">How the name of the xref should appear in the new document</param>
        /// <param name="layoutname">Space where the xref should be added; can only be Model or PaperSpace</param>
        public static void Xref_Attach ( this Document doc, string xref_fullpath, string xref_name, string layoutname)
        {
            Database db = doc.Database;
            using (DocumentLock doclock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    if (File.Exists(xref_fullpath))
                    {
                        ObjectId xrefid = new ObjectId(IntPtr.Zero);
                        Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "x-xref");
                        doc.LayerManagement("Lock", "x-xref", false);
                        try
                        {
                            xrefid = db.AttachXref(xref_fullpath, xref_name);
                        }

                        catch (Autodesk.AutoCAD.Runtime.Exception aex)
                        {
                            if (aex.ErrorStatus == ErrorStatus.FileAccessErr)
                            {
                                doc.Editor.WriteMessage("The following drawing is likely a new drawing and cannot be setup");
                            }
                        }

                        if (!xrefid.IsNull)
                        {
                            Point3d origin_default = new Point3d(0, 0, 0);
                            BlockReference xref_block = new BlockReference(origin_default, xrefid);
                            List<Layout> list_lays = Project_Setup.ProjectSetup.Project_ListOfLayouts(doc);
                            if (layoutname.ToUpper().Equals("MODEL"))
                            {
                                ObjectId modelspaceid = (from l in list_lays
                                                            where l.LayoutName.ToUpper().Equals("MODEL")
                                                            select l.OwnerId).First();
                                //BlockTableRecord mspacebtr = tr.GetObject(modelspaceid, OpenMode.ForWrite) as BlockTableRecord;
                                BlockTable blt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                                BlockTableRecord mspacebtr = tr.GetObject(blt[BlockTableRecord.ModelSpace] , OpenMode.ForWrite) as BlockTableRecord;
                                mspacebtr.AppendEntity(xref_block);
                                tr.AddNewlyCreatedDBObject(xref_block, true);
                            }

                            else
                            {
                                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "x-tblk");
                                if (list_lays.Any(ln => ln.LayoutName.Equals(layoutname)))
                                {
                                    //ObjectId otherspaceid = (from l in list_lays
                                    //                         where l.LayoutName.ToUpper().Equals(layoutname)
                                    //                         select l.OwnerId).First();
                                    BlockTable blt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                                    BlockTableRecord otherspacebtr = tr.GetObject(blt[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;
                                    otherspacebtr.AppendEntity(xref_block);
                                    tr.AddNewlyCreatedDBObject(xref_block, true);
                                }

                                else
                                {
                                    LayoutManager laymgr = LayoutManager.Current;
                                    ObjectId newlayoutid = laymgr.CreateLayout(layoutname);
                                    Layout newlayout = tr.GetObject(newlayoutid, OpenMode.ForRead) as Layout;
                                    BlockTableRecord newlayoutbtr = tr.GetObject(newlayout.OwnerId, OpenMode.ForRead) as BlockTableRecord;
                                    newlayoutbtr.AppendEntity(xref_block);
                                    tr.AddNewlyCreatedDBObject(xref_block, true);
                                }
                            }
                            tr.Commit();
                        }
                    }
                }
            }
        }

        public static Layout Create_NewLayout(this Document doc, string layoutname)
        {
            Layout layout = new Layout();

            using (DocumentLock doclck = doc.LockDocument())
            {
                using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
                {
                    LayoutManager laymgr = LayoutManager.Current;
                    ObjectId layoutID = new ObjectId();

                    try
                    {
                        layoutID = laymgr.CreateLayout(layoutname);
                    }
                        
                    catch (Autodesk.AutoCAD.Runtime.Exception aex)
                    {
                        if (aex.ErrorStatus == ErrorStatus.DuplicateKey)
                        {
                            layoutID = laymgr.CreateLayout(layoutname + "(1)");
                        }
                    }

                    layout = tr.GetObject(layoutID, OpenMode.ForWrite) as Layout;
                    BlockTableRecord lay_btr = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForWrite) as BlockTableRecord;

                    laymgr.CurrentLayout = layout.LayoutName;

                    ObjectId layout_btrid = layout.OwnerId;
                    ObjectIdCollection vp_ids = layout.GetViewports();
                    RXClass VPClass = RXObject.GetClass(typeof(Viewport));
                    foreach (ObjectId item in lay_btr)
                    {
                        if (item.ObjectClass == VPClass)
                        {
                            Viewport vp = tr.GetObject(item, OpenMode.ForWrite) as Viewport;
                            if (vp.Number != 1)
                            {
                                vp.Erase();
                            }

                            doc.Editor.Regen();
                        }
                    }  

                    tr.Commit();
                }
            }
            return layout;
        }
        /// <summary>
        /// Gets the main BlockTableRecord for the document, does not refer to a specific layout
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="openmode"></param>
        /// <returns></returns>
        public static BlockTableRecord GetPaperSpaceRecord(this Document doc, OpenMode openmode, DocumentLock doclock, Transaction tr)
        {
            Database db = doc.Database;
            BlockTableRecord btr_paper;

            using (doclock)
            {
                using (tr)
                {
                    BlockTable blt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    btr_paper = tr.GetObject(blt[BlockTableRecord.PaperSpace], openmode) as BlockTableRecord;
                }
            }

            return btr_paper;
        }

        public static void LayerManagement(this Document doc, string Operation, string sLayerName, bool yesno)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            switch(Operation)
            {
                case "Lock":
                    #region Locking Code
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Open the Layer table for read
                        LayerTable acLyrTbl;
                        acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                        OpenMode.ForRead) as LayerTable;

                        if (acLyrTbl.Has(sLayerName) == false)
                        {
                            using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                            {
                                // Assign the layer a name
                                acLyrTblRec.Name = sLayerName;

                                // Upgrade the Layer table for write
                                acLyrTbl.UpgradeOpen();

                                // Append the new layer to the Layer table and the transaction
                                acLyrTbl.Add(acLyrTblRec);
                                acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);

                                // Lock the layer
                                acLyrTblRec.IsLocked = yesno;
                            }
                        }
                        else
                        {
                            LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName],
                                                            OpenMode.ForWrite) as LayerTableRecord;

                            // Lock the layer
                            acLyrTblRec.IsLocked = yesno;
                        }

                        // Save the changes and dispose of the transaction
                        acTrans.Commit();
                        break;
                    }
                    #endregion
                case "Freeze":
                    #region Freeze Code
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Open the Layer table for read
                        LayerTable acLyrTbl;
                        acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                        OpenMode.ForRead) as LayerTable;

                        if (acLyrTbl.Has(sLayerName) == false)
                        {
                            using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                            {
                                // Assign the layer a name
                                acLyrTblRec.Name = sLayerName;

                                // Upgrade the Layer table for write
                                acLyrTbl.UpgradeOpen();

                                // Append the new layer to the Layer table and the transaction
                                acLyrTbl.Add(acLyrTblRec);
                                acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);

                                // Freeze the layer
                                acLyrTblRec.IsFrozen = yesno;
                            }
                        }
                        else
                        {
                            LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName],
                                                            OpenMode.ForWrite) as LayerTableRecord;

                            // Freeze the layer
                            acLyrTblRec.IsFrozen = yesno;
                        }

                        // Save the changes and dispose of the transaction
                        acTrans.Commit();
                    }
                    break;
                    #endregion
                default:
                break;
            }
        }
    }
 }
    
