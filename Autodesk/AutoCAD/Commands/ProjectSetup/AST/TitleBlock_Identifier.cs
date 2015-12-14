using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ExtensionMethods;
using Project_Setup;


namespace TitleBlock
{
    /// <summary>
    /// An attempt at trying to find a way to distinguish a specific block reference 
    /// as a TitleBlock - look to try and make this an extension of a Layout instead once its figured out
    /// </summary>
    public class TitleBlock_Identifier
    {
        static void Main()
        {
            //Access and lock the current document and database
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            DocumentLock lckdoc = doc.LockDocument();
            Editor ed = doc.Editor;

            //Gather all the xrefs from the document, if there are any
            List<BlockTableRecord> xlist = ProjectSetup.Xref_List(doc);

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
                    
                    var l_name = l.LayoutName;
                    var layout_extents = l.Extents;
                    var layout_limits = l.Limits;
                    ObjectId lbtr_id = l.BlockTableRecordId;
                    BlockTableRecord l_btr = tr.GetObject(lbtr_id, OpenMode.ForRead) as BlockTableRecord;
                        
                    List<ObjectId> objs_inLayout = new List<ObjectId>();
                    foreach (ObjectId x in l_btr)
                    {
                        objs_inLayout.Add(x);
                    }

                    int o = objs_inLayout.Count();

                    foreach (BlockTableRecord x in xlist)
                    {
                        ObjectIdCollection BlockRefIdsColl = x.GetBlockReferenceIds(true, true);
                        List<ObjectId> BRI_list = new List<ObjectId>();
                        foreach (ObjectId objid in BlockRefIdsColl)
                        {
                            BRI_list.Add(objid);
                        }
                        ObjectId BlockId = BRI_list[0];
                        BlockReference blref =
                            tr.GetObject(BlockId, OpenMode.ForRead) as BlockReference;
                        var block_bounds = blref.Bounds;
                        var block_geoextents = blref.GeometricExtents;
                        //var block_xtra = blref.Wb

                }
            }
        }
        }
    }
}
