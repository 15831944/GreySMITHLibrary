using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Windows;
using Autodesk.AutoCAD.Windows;
using ExtensionMethods;
using Cleaners;
using PageSetup;

#region Other Classes
namespace Project_Setup
{
    public class Xref_Info : General_Setup.DictOfLists
    {
        public string XrOrigin;

        public Xref_Info(string xrname, string xrpath, string xreforigin)
        {
            //adds the parameters info to the properties of the object
            FileName = xrname;
            FilePath = xrpath;
            XrOrigin = xreforigin;

            // pushes all the info into a list which will
            // later be pushed into a dictionary
            List<string> xinfo = Xref_Info.FillNewList(FileName, FilePath, XrOrigin);

            //Adds all the info together and assigns a number to each
            this.Add(IdentityNumbers, xinfo);
        }
    }

    public class DictOfViewPorts : Dictionary<Layout, List<Viewport>>
    {
        public int IdentityNumbers;
        public static int TotalNumIdens;
        public Layout Layout;

        public DictOfViewPorts()
        {
            TotalNumIdens++;
            IdentityNumbers = TotalNumIdens;
        }

        public DictOfViewPorts(Viewport vpinfo)
        {
            TotalNumIdens++;
            IdentityNumbers = TotalNumIdens;
            List<Viewport> list_vpinfo = new List<Viewport>();
            list_vpinfo.Add(vpinfo);
            this.Add(Layout, list_vpinfo);
        }
    }
#endregion
    #region Fields
    public class ProjectSetup
    {

        /// <summary>
        /// Parameter which will describe the kind
        /// of methodology which should
        /// be used in order to set up the files
        /// </summary>
        private static string File_Setup_Type = null;
    #endregion
        #region Project Setup Methods
        /// <summary>
        /// Returns a list of the BlockTableRecords in the current Document's BlockTable using their ObjectIDs
        /// </summary>
        /// <returns> A List of all the block table records in the current project </returns>
        public static List<BlockTableRecord> GetAllBtrs()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
        ///Gets a List of BlockTableRecords that are External References 
        ///</summary>
        ///<param name="database"> The database you want to query </param>
        ///<param name="doc"> The document you want to retrieve a list of Xrefs from </param>
        public static List<BlockTableRecord> Xref_List(Document doc)
        {
            //Create a list of all xrefs in the file
            List<BlockTableRecord> tempxrlist;
            List<BlockTableRecord> XList = new List<BlockTableRecord>();

            //Check the drawing to see if there are any xrefs
            using (Transaction Tran = doc.TransactionManager.StartTransaction())
            {
                //Get the Block table record for the entire file
                tempxrlist = ProjectSetup.GetAllBtrs();

                //Make a list of all the Xrefs in the file
                foreach (BlockTableRecord a in tempxrlist)
                {
                    if (a.IsFromExternalReference)
                    {
                        XList.Add(a);
                    }
                }
                //Complete the command
                Tran.Commit();
            }
            return XList;
        }
        /// <summary>
        /// Subtracts one string from a larger string
        /// </summary>
        /// <param name="x"> String to delete from </param>
        /// <param name="y"> String to be deleted </param>
        /// <returns> A string value </returns>
        public static string StrSubtract(string x, string y)
        {
            //Takes two strings and subtracts the second value from the first
            string z = x.TrimEnd(y.ToCharArray());
            return z;
        }
        /// <summary> 
        /// Exports a BlockTableRecord out to another location using a WBlock methodology 
        /// </summary>
        /// <param name="btr"> The External Reference to be exported; must be a Block Table Record </param>
        public static void Xref_Exporter(BlockTableRecord btr)
        {
            //Access the current document
            Document Cdoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database Adbase = Cdoc.Database;

            //***Gets the current directory by finding the file path
            string FilePath = Path.GetDirectoryName(Adbase.Filename);

            //Creates a new directory for all files to be saved to
            //called "_Setup Files" and "Xrefs"
            string NewFol = "_Setup Files";
            string NewSetup = "SETUP";
            string NewXrPath = null;
            string XrPath = btr.PathName;
            string XrOrigin = btr.Origin.ToString();
            string XrName = btr.Name.ToString().ToUpper();
            General_Setup.Setup_Methods.Directory_Creator(FilePath, NewFol, NewSetup);

            //if the Xref doesn't already exist in a xref/setup folder export it there
            if (!File.Exists(Path.Combine(FilePath, NewFol, NewSetup, XrName)))
            {
                NewXrPath = Path.Combine(FilePath, NewFol, NewSetup);
                NewXrPath = Path.Combine(NewXrPath, XrName);

                try
                {
                    // if the xref in question can't be found in CAD
                    // i.e. because the xref is set to Absolute instead of Relative
                    // or because the xref is in another place for some reason
                    // search in all directories and try to resolve the xref
                    if (!btr.IsResolved)
                    {
                        Adbase.ResolveXrefs(true, false);
                    }

                    //if the xref has been found
                    if (btr.IsResolved)
                    {
                        //writes the xref out to a base location with its same name
                        using (Transaction tr = Adbase.TransactionManager.StartTransaction())
                        {
                            using (Database newDb = btr.GetXrefDatabase(true))
                            {
                                ObjectIdCollection idCol = new ObjectIdCollection();
                                IdMapping idMap = new IdMapping();
                                Adbase.WblockCloneObjects(idCol, btr.ObjectId, idMap, DuplicateRecordCloning.Ignore, false);
                                tr.Commit();
                                if (!Directory.Exists(NewXrPath))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(NewXrPath));
                                }

                                //adds an .dwg to the end of the path if there isn't any
                                if (!Path.HasExtension(NewXrPath))
                                {
                                    NewXrPath += ".dwg";
                                }
                                newDb.SaveAs(NewXrPath, DwgVersion.AC1021);
                            }
                        }
                    }
                    else
                    {
                        // Write code to export the name of the file that is necessary out to the Xrefs area
                        // in a txt file preferably
                        Editor ed = Cdoc.Editor;
                        ed.WriteMessage("\n {0} was not found, ask the client to send it.", btr.Name);
                    }
                }

                catch (Autodesk.AutoCAD.Runtime.Exception aex)
                {
                    if (aex.ErrorStatus == ErrorStatus.InvalidKey)
                    {
                        // Write code to export the name of the file that is necessary out to the Xrefs area
                        // in a txt file preferably
                        Editor ed = Cdoc.Editor;
                        ed.WriteMessage("\n {0} was not found, ask the client to send it.", btr.Name);
                    }
                }
            }
        }
        ///<summary>
        /// Copies an External Reference from an existing location to another folder
        /// will overwrite any file with the same name
        ///</summary>
        public static void Xref_ExporterCopy(BlockTableRecord x)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //***Gets the current directory by finding the file path
            string curloc = Path.GetDirectoryName(db.Filename);

            //Creates a new directory for all files to be saved to
            //called "_Setup Files" and "Xrefs"
            string NewFol = "_Setup Files";
            string coploc = System.IO.Path.Combine(curloc, NewFol);

            //Creates the directory if it doesn't already exist
            General_Setup.Setup_Methods.Directory_Creator(coploc);

            if (!File.Exists(Path.Combine(curloc, NewFol, x.Name)))
            {
                // make a list of all the drawing files in every directory 
                // in the path leading up the directory the main file lives
                try
                {
                    string[] allpossiblexrefs = Directory.EnumerateFiles(
                        (Path.GetDirectoryName(curloc)),
                        "*.dwg",
                        SearchOption.AllDirectories).ToArray<string>();
                    // if the file is in the array
                    // copy the file from its location out to copy location

                    var copy_xref = from file in allpossiblexrefs
                                    where Path.GetFileName(file).Equals((x.Name + ".dwg"))
                                    select file;
                    if (copy_xref != null)
                    {
                        foreach (var file in copy_xref)
                        {
                            File.Copy(file, (Path.Combine(coploc, (x.Name + ".dwg"))), true);
                            break;
                        }
                    }
                    else
                    {
                        Xref_Exporter(x);
                    }
                }

                catch (DirectoryNotFoundException)
                {
                    //remember to add some code here and figure out
                    //how to look through all the directories better

                    // If there were any xrefs that weren't within the main directory
                    // this method will Wblock them out to a base location
                    Xref_Exporter(x);
                }
            }
            else
            {
                Editor ed = doc.Editor;
                ed.WriteMessage("\n {0} has already been exported to the Xrefs folder.", x.Name);
            }
        }
        ///<summary> 
        ///Finds the paths of External References that are currently unresolved 
        ///</summary>
        ///<param name="x"> An External Reference with an unresolved path; must be a block table record</param>
        public static void Xref_PathFixer(BlockTableRecord x)
        {
            //make a list of all the drawing files in every directory 
            //in the path leading up the directory the main file lives
            string[] result = Directory.EnumerateFiles(x.PathName, ".dwg", SearchOption.AllDirectories).ToArray<string>();

            //if the file is in the array
            //change the xrpath to its location instead
            if (result.Contains(x.Name))
            {
                var xrefs = from file in result
                            where file.Equals(x.PathName)
                            select file;

                foreach (var file in xrefs)
                {
                    if (file.Equals(x.PathName))
                    {
                        Field FilePath = new Field(file.ToString());
                        Xref_FieldChanger(x, "PathName", FilePath);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Deletes any empty layouts in the current file
        /// </summary>
        public static void Layout_Purifier(Document doc)
        {
            Database db = doc.Database;
            List<Layout> listoflayouts = Project_ListOfLayouts(doc);
            List<ObjectId> layouts = Project_ListOfLayoutIds(doc);


            // if there is more than one layout
            // look through all to see which are empty
            // and delete each of them

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                if (layouts.Count() < 1)
                {
                    List<ObjectId> objs_inLayout = new List<ObjectId>();
                    foreach (Layout layout in listoflayouts)
                    {
                        ObjectId layoutbtrid = layout.BlockTableRecordId;
                        BlockTableRecord layoutbtr = trans.GetObject(layoutbtrid, OpenMode.ForRead) as BlockTableRecord;
                        foreach (ObjectId x in layoutbtr)
                        {
                            objs_inLayout.Add(x);
                        }
                        if (objs_inLayout.Count() < 1)
                        {
                            // Delete layout
                            LayoutManager.Current.DeleteLayout(layout.LayoutName);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// //STILL NEEDS TO BE CHECKED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// should make it easier to change the data inside
        /// of any External Reference
        /// </summary>
        /// <param name="obj">External Reference whose information should be changed</param>
        /// <param name="field_name">Name of the Property which should be changed</param>
        /// <param name="field_value_new">The actual Value user wants the Property to have</param>
        public static void Xref_FieldChanger(BlockTableRecord obj, string field_name, Field field_value_new)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (!obj.HasFields)
                {
                    ed.WriteMessage("\n{0} does not contain fields.", obj.Name);
                    return;
                }

                // Gets all the properties in the object
                // and allows to user to place them in an array
                ObjectId id = obj.GetField();
                Field fieldhldr = tr.GetObject(id, OpenMode.ForRead) as Field;
                Field[] fields = fieldhldr.GetChildren();

                // Finds the parameter whose name matches
                //"field_name" and allows the user to change its value
                var childField = from field in fields
                                 where field.Equals(field_name)
                                 select field;
                foreach (var field in childField)
                {
                    if (field.Equals(field_name))
                    {
                        string fldCode =
                                        field.GetFieldCode(
                                        FieldCodeFlags.AddMarkers |
                                        FieldCodeFlags.FieldCode);
                        obj.SetField(fldCode, field_value_new);
                        break;
                    }
                }
                tr.Commit();
            }
        }
        /// <summary>
        /// FIX ALL OF THESE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Collects info on a viewport; using a TitleBlock's Object ID this
        /// finds and all viewports in the same Layout and returns their relevant info
        /// </summary>
        /// <param name="tblk">TitleBlock which should be used</param>
        public static List<Viewport> Xref_ViewPortInfo(ObjectId tblk_layid)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            List<Viewport> list_vpinfo = new List<Viewport>();

            if (!tblk_layid.IsNull)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Layout curlay =
                        tr.GetObject(tblk_layid, OpenMode.ForRead) as Layout;

                    if (!curlay.ModelType)  //if the layout is not a modelspace layout
                    {
                        string layname = curlay.LayoutName;
                        ObjectIdCollection vports = curlay.GetViewports();
                        int numofvps = 0;

                        foreach (ObjectId v in vports)
                        {
                            numofvps++;
                            Viewport vp = v.GetObject(OpenMode.ForRead) as Viewport;

                            Viewport vpinfo =
                                new Viewport(
                                    vp.StandardScale,
                                    vp.CustomScale,
                                    vp.CenterPoint,
                                    vp.ViewCenter,
                                    vp.Width,
                                    vp.Height,
                                    vp.ViewHeight,
                                    vp.ViewTarget,
                                    vp.GetFrozenLayers());

                            list_vpinfo.Add(vpinfo);
                        }
                    }
                }
            }
            return list_vpinfo;
        }
        /// <summary>
        /// Returns a list of ObjectIds representing the layouts present in the document
        /// as well as a list of the actual layout objects themselvess
        /// </summary>
        /// <param name="doc">Document which should be checked for layouts</param>
        /// <returns>A list of ObjectIds of the Layouts' BlockTableRecords</returns>
        public static List<ObjectId> Project_ListOfLayoutIds(Document doc)
        {
            List<ObjectId> listoflayids = new List<ObjectId>();
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                List<BlockTableRecord> btrlist = GetAllBtrs();
                IEnumerable<BlockTableRecord> laybtrs = from L_id in btrlist
                                                        where L_id.IsLayout
                                                        select L_id;

                foreach (BlockTableRecord laybtr in laybtrs)
                {
                    listoflayids.Add(laybtr.LayoutId);
                }
            }
            return listoflayids;
        }

        public static List<Layout> Project_ListOfLayouts(Document doc)
        {
            List<ObjectId> listoflayids = new List<ObjectId>();
            List<Layout> listoflayouts = new List<Layout>();
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                List<BlockTableRecord> btrlist = GetAllBtrs();
                IEnumerable<BlockTableRecord> laybtrs = from L_id in btrlist
                                                        where L_id.IsLayout
                                                        select L_id;

                foreach (BlockTableRecord laybtr in laybtrs)
                {
                    listoflayids.Add(laybtr.LayoutId);
                }

                foreach (ObjectId layid in listoflayids)
                {
                    Layout l = tr.GetObject(layid, OpenMode.ForRead) as Layout;
                    listoflayouts.Add(l);
                }
                return listoflayouts;
            }

        }

        public static ObjectId[] Xref_LayoutFinder(BlockTableRecord ExternalReference)
        {
            using (Transaction tr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                //Get all the BlockReferences relative to the BlockTableRecord in question
                //create a list to cycle through their respective ObjectIds
                ObjectIdCollection BlockRefIdsColl = ExternalReference.GetBlockReferenceIds(true, true);
                List<ObjectId> BRI_list = new List<ObjectId>();
                foreach (ObjectId objid in BlockRefIdsColl)
                {
                    BRI_list.Add(objid);
                }

                //if there's only one Id in the list
                if (BRI_list.Count() == 1)
                {
                    ObjectId BlockId = BRI_list[0];
                    BlockReference blref =
                        tr.GetObject(BlockId, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord layoutbtr =
                        tr.GetObject(blref.OwnerId, OpenMode.ForRead) as BlockTableRecord;
                    Layout ltest =
                        tr.GetObject(layoutbtr.LayoutId, OpenMode.ForRead) as Layout;
                    string layname = ltest.LayoutName;

                    ObjectId[] xref_layid = new ObjectId[] { ltest.ObjectId };
                    return xref_layid;
                }

                else
                {
                    List<ObjectId> layout_ids = new List<ObjectId>();
                    foreach (ObjectId objid in BlockRefIdsColl)
                    {
                        BRI_list.Add(objid);
                    }
                    //if there's more than one id but they're all the same
                    if (BRI_list.Distinct().Count() == 1)
                    {
                        ObjectId BlockId = BRI_list[0];
                        BlockReference blref =
                            tr.GetObject(BlockId, OpenMode.ForRead) as BlockReference;
                        BlockTableRecord layoutbtr =
                            tr.GetObject(blref.OwnerId, OpenMode.ForRead) as BlockTableRecord;
                        Layout ltest =
                            tr.GetObject(layoutbtr.LayoutId, OpenMode.ForRead) as Layout;
                        string layname = ltest.LayoutName;

                        ObjectId[] xref_layid = new ObjectId[] { ltest.ObjectId };
                        return xref_layid;
                    }
                    //if there's more than one distinct Id
                    else
                    {
                        List<ObjectId> xref_layids = new List<ObjectId>();
                        foreach (ObjectId tempBlockId in BRI_list)
                        {
                            BlockReference blref =
                            tr.GetObject(tempBlockId, OpenMode.ForRead) as BlockReference;
                            BlockTableRecord layoutbtr =
                                tr.GetObject(blref.OwnerId, OpenMode.ForRead) as BlockTableRecord;
                            Layout ltest =
                                tr.GetObject(layoutbtr.LayoutId, OpenMode.ForRead) as Layout;
                            string layname = ltest.LayoutName;

                            ObjectId tempxref_layid = ltest.ObjectId;
                            xref_layids.Add(tempxref_layid);
                        }

                        return xref_layids.ToArray();
                    }
                }
            }
        }

        #endregion

        #region File_Setup_Analysis
        [CommandMethod("ProjSet", CommandFlags.Session)]
        public static void Main()
        {
            DocumentCollection mgr_doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            //Access and lock the current document and database
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            HostApplicationServices.WorkingDatabase = db;

            doc.NET_AUDIT();
            doc.NET_PURGE();

            // if the drawing has any AEC or Proxy objects, immediately
            // send out to AECTOACAD, audit, purge and then start work
            
            //doc.SendStringToExecute(
            //    "AECTOACAD",
            //    true,
            //    true,
            //    true);

            //Setup directories
            string curloc = Path.GetDirectoryName(db.Filename);

            //Creates a new directory for all files to be saved to
            //called "_Setup Files" and "Xrefs"
            string NewFol = "_Setup Files";
            string NewSetups = "SETUP";
            string setuploc = System.IO.Path.Combine(curloc, NewFol, NewSetups);
            string xrefloc = System.IO.Path.Combine(curloc, NewFol);

            //creates the setup directories if they don't already exist
            if (!Directory.Exists(xrefloc)) { Directory.CreateDirectory(xrefloc); }
            if (!Directory.Exists(setuploc)) { Directory.CreateDirectory(setuploc); }

            //Gather all the xrefs from the document, if there are any
            List<BlockTableRecord> xlist = Xref_List(doc);

            //Get a list of layouts/layoutids
            List<Layout> listoflayouts = Project_ListOfLayouts(doc);
            List<ObjectId> listoflayids = Project_ListOfLayoutIds(doc);

            // The objectIDs for the BlockTableRecords of the Model and PaperSpace layouts
            ObjectId mspace = listoflayids[0];
            List<ObjectId> pspaces = listoflayids.GetRange(1, (listoflayids.Count() - 1));
            DictOfViewPorts vp_dict = new DictOfViewPorts();
    #endregion

            #region External Reference Exportation
            using (DocumentLock doclock = doc.LockDocument())
            {
                //Start the process of saving out each of the unique
                //xrefs into another folder called "Xrefs"
                if (xlist.Count() > 0)
                {
                    foreach (BlockTableRecord x in xlist)
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            //if the file already exists somewhere else in the directory
                            //just copy it from there to the base location instead
                            Xref_ExporterCopy(x);
                        }
                    }
                }
            #endregion

                #region Viewport Information
                //get the info on all viewports in all layouts
                foreach (Layout layout in listoflayouts)
                {
                    List<Viewport> list_vpinfo = Xref_ViewPortInfo(layout.ObjectId);
                    vp_dict.Add(layout, list_vpinfo);
                }
            }
                #endregion

                #region Actual Setup File Creation
            #region Misc. Saving
            //doc is no longer locked
            //save the consultant drawing into the xref folder with its references
            try
            {
                doc.Database.SaveAs(
                    (Path.Combine(xrefloc, (Path.GetFileName(db.Filename)))),
                    true,
                    DwgVersion.AC1800,
                    db.SecurityParameters);
            }

            catch (Autodesk.AutoCAD.Runtime.Exception aex)
            {
                if (aex.ErrorStatus == ErrorStatus.FileAccessErr)
                {
                    // error handling code
                }
            }

            catch
            {
                // error handling code
            }
            #endregion
            #region Xref Insertion & Layout Creation
            foreach (Layout l in listoflayouts)
            {
                if (!l.ModelType)
                {
                    //create a new dwg based on the BR+A template
                    //switch the active drawing to the newly made drawing
                    string BRA = "G:\\Shared\\CADD\\ACAD2011\\Template\\BR+A.dwt";
                    Document newdoc = doc.CreateNew(BRA, setuploc, l.LayoutName);
                    Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = newdoc;
                    HostApplicationServices.WorkingDatabase = newdoc.Database;
                    try
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "x-xref");
                        newdoc.LayerManagement("Lock", "x-xref", false);
                    }
                    catch
                    {
                        // error handling code
                    }

                    //Insert the consultant drawing at 0,0,0
                    newdoc.Xref_Attach(
                        (doc.Name),
                        Path.GetFileNameWithoutExtension(doc.Name),
                        "Model");

                    newdoc.Editor.Regen();

                    //insert the default titleblock based on the size of the previous document's layout
                    string tblksize;
                    int numoflays = 0;
                    string tblk_dir = Path.GetDirectoryName("G:\\Shared\\CADD\\BLOCKLIB\\BR&A\\NYC\\BR+A-NY.dwg");
                    string TBLK = Path.Combine(xrefloc, "TBLK.dwg");

                    using (DocumentLock d_lock = newdoc.LockDocument())
                    {
                        using (Transaction tr1 = newdoc.Database.TransactionManager.StartTransaction())
                        {
                            numoflays++;
                            tblksize = l.PaperSize();

                            // If the layout doesn't already exist in the drawing, create it
                            LayoutManager newlaymgr = LayoutManager.Current;
                            List<Layout> new_listoflayouts = Project_ListOfLayouts(newdoc);
                            Layout curlay = new Layout();
                            PlotPaperUnit ppunits = PlotPaperUnit.Inches;
                            if (!new_listoflayouts.Any(ln => ln.LayoutName.Equals(l.LayoutName)))
                            {
                                curlay = newdoc.Create_NewLayout(l.LayoutName);
                            }

                            else
                            {
                                curlay = new_listoflayouts[new_listoflayouts.FindIndex(ln => ln.LayoutName.Equals(l.LayoutName))];
                            }
            #endregion
                            #region Title Block Switch Statements
                            // case statements pick out specific titleblocks
                            try
                            {
                                switch (tblksize)
                                {
                                    case "11,17":
                                        //change the actual page size
                                        curlay.SetPageSize(11.00, 17.00, ppunits);

                                        // save the titleblock from the G Drive into the project folder
                                        // as TBLK.dwg and the titleblock info as well as TBLKinfo.dwg
                                        if (!File.Exists(TBLK))
                                        {
                                            File.Copy((Path.Combine(tblk_dir, "11X17.dwg")), (Path.Combine(xrefloc, "TBLK.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "11X17INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                                        }

                                        else
                                        {
                                            string other_tblk = Path.Combine(xrefloc, "TBLK_11X17.dwg");
                                            File.Copy((Path.Combine(tblk_dir, "11X17.dwg")), (Path.Combine(xrefloc, "TBLK_11X17.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "11X17INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo_11X17.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(other_tblk, "TBLK_11X17", l.LayoutName);
                                        }
                                        break;
                                    case "24,36":
                                        //change the actual page size
                                        curlay.SetPageSize(24.00, 36.00, ppunits);

                                        // save the titleblock from the G Drive into the project folder
                                        // as TBLK.dwg and the titleblock info as well as TBLKinfo.dwg
                                        if (!File.Exists(TBLK))
                                        {
                                            File.Copy((Path.Combine(tblk_dir, "24X36.dwg")), (Path.Combine(xrefloc, "TBLK.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "24X36INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                                        }

                                        else
                                        {
                                            string other_tblk = Path.Combine(xrefloc, "TBLK_24X36.dwg");
                                            File.Copy((Path.Combine(tblk_dir, "24X36.dwg")), (Path.Combine(xrefloc, "TBLK_24X36.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "24X36INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo_24X36.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(other_tblk, "TBLK_24X36", l.LayoutName);
                                        }
                                        break;
                                    case "30,42":
                                        //change the actual page size
                                        curlay.SetPageSize(30.00, 42.00, ppunits);

                                        // save the titleblock from the G Drive into the project folder
                                        // as TBLK.dwg and the titleblock info as well as TBLKinfo.dwg
                                        if (!File.Exists(TBLK))
                                        {
                                            File.Copy((Path.Combine(tblk_dir, "30X42.dwg")), (Path.Combine(xrefloc, "TBLK.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "30X42INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                                        }

                                        else
                                        {
                                            string other_tblk = Path.Combine(xrefloc, "TBLK_30X42.dwg");
                                            File.Copy((Path.Combine(tblk_dir, "30X42.dwg")), (Path.Combine(xrefloc, "TBLK_30X42.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "30X42INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo_30X42.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(other_tblk, "TBLK_30X42", l.LayoutName);
                                        }
                                        break;
                                    case "30,48":
                                        //change the actual page size
                                        curlay.SetPageSize(30.00, 48.00, ppunits);

                                        // save the titleblock from the G Drive into the project folder
                                        // as TBLK.dwg and the titleblock info as well as TBLKinfo.dwg
                                        if (!File.Exists(TBLK))
                                        {
                                            File.Copy((Path.Combine(tblk_dir, "30X48.dwg")), (Path.Combine(xrefloc, "TBLK.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "30X48INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                                        }

                                        else
                                        {
                                            string other_tblk = Path.Combine(xrefloc, "TBLK_30X48.dwg");
                                            File.Copy((Path.Combine(tblk_dir, "30X48.dwg")), (Path.Combine(xrefloc, "TBLK_30X48.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "30X48INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo_30X48.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(other_tblk, "TBLK_30X48", l.LayoutName);
                                        }
                                        break;
                                    default:
                                    case "36,48":
                                        //change the actual page size
                                        curlay.SetPageSize(36.00, 48.00, ppunits);

                                        // save the titleblock from the G Drive into the project folder
                                        // as TBLK.dwg and the titleblock info as well as TBLKinfo.dwg
                                        if (!File.Exists(TBLK))
                                        {
                                            File.Copy((Path.Combine(tblk_dir, "36X48.dwg")), (Path.Combine(xrefloc, "TBLK.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "36X48INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                                        }

                                        else
                                        {
                                            string other_tblk = Path.Combine(xrefloc, "TBLK_36X48.dwg");
                                            File.Copy((Path.Combine(tblk_dir, "36X48.dwg")), (Path.Combine(xrefloc, "TBLK_36X48.dwg")), false);
                                            File.Copy((Path.Combine(tblk_dir, "36X48INF.dwg")), (Path.Combine(xrefloc, "TBLKinfo_36X48.dwg")), false);

                                            //insert the titleblock
                                            newdoc.Xref_Attach(other_tblk, "TBLK_36X48", l.LayoutName);
                                        }
                                        break;
                                }
                            }

                            catch (IOException)
                            {
                                //change the actual page size
                                curlay.SetPageSize(36.00, 48.00, ppunits);

                                //insert the titleblock
                                newdoc.Xref_Attach(TBLK, "TBLK", l.LayoutName);
                            }
                            //newdoc.Editor.Regen();
                            
                            tr1.Commit();
                        }
                            #endregion
                        #region Layout Creation

                        //switches to appropiate layout and calls it "Work"
                        using (Transaction tr2 = mgr_doc.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                        {
                            LayoutManager newlaymgr = LayoutManager.Current;
                            Layout curlay = new Layout();

                            List<Layout> new_listoflayouts = Project_ListOfLayouts(newdoc);
                            if (new_listoflayouts.Any(ln => ln.LayoutName.Equals(l.LayoutName)))
                            {
                                for (int i = 0; i < new_listoflayouts.Count; i++)
                                {
                                    if (new_listoflayouts[i].LayoutName.Equals(l.LayoutName))
                                    {
                                        curlay = new_listoflayouts[i];
                                        break;
                                    }
                                }
                            }

                            else
                            {
                                newdoc.Create_NewLayout(l.LayoutName);
                                for (int i = 0; i < new_listoflayouts.Count; i++)
                                {
                                    if (new_listoflayouts[i].LayoutName.Equals(l.LayoutName))
                                    {
                                        curlay = new_listoflayouts[i];
                                        break;
                                    }
                                }
                            }

                            if (curlay.LayoutName.ToUpper() != "LAYOUT1")
                            {
                                newlaymgr.DeleteLayout("Layout1");
                            }

                            newlaymgr.RenameLayout(curlay.LayoutName, "Work");

                            // for each set of viewport info in the List of VP info
                            foreach (Viewport vpinfo in vp_dict[l])
                            {
                                // create a new vp with the corresponding info
                                curlay.Viewport_Create(vpinfo, newdoc);
                                //newdoc.Editor.Regen();
                            }
                            tr2.Commit();
                        }
                        newdoc.Editor.Regen();

                        //delete the extra viewport that gets created sometimes...
                        using (Transaction tr3 = newdoc.TransactionManager.StartTransaction())
                        {
                            LayoutManager newlaymgr1 = LayoutManager.Current;
                            Layout layout = tr3.GetObject(newlaymgr1.GetLayoutId(newlaymgr1.CurrentLayout), OpenMode.ForWrite) as Layout;
                            BlockTableRecord lay_btr = tr3.GetObject(layout.BlockTableRecordId, OpenMode.ForWrite) as BlockTableRecord;

                            ObjectIdCollection vp_ids = layout.GetViewports();
                            RXClass VPClass = RXObject.GetClass(typeof(Viewport));
                            foreach (ObjectId item in lay_btr)
                            {
                                if (item.ObjectClass == VPClass)
                                {
                                    Viewport vp = tr3.GetObject(item, OpenMode.ForWrite) as Viewport;
                                    var laywidth = layout.GetWidth();
                                    var layheight = layout.GetHeight();
                                    if (vp.Number != 1 && (vp.Height > layheight || vp.Width > laywidth))
                                    {
                                        vp.Erase();
                                    }
                                    newdoc.Editor.Regen();
                                }
                            }
                            tr3.Commit();
                        }
                    }
                            #endregion
                    #region Closing Area
                    try
                    {
                        newdoc.NET_AUDIT();
                        newdoc.NET_PURGE();

                        //save all the changes
                        newdoc.Database.SaveAs(
                            (newdoc.Name),
                            true,
                            DwgVersion.AC1800,
                            db.SecurityParameters);
                    }

                    catch
                    {
                        break;
                    }

                    try
                    {
                        mgr_doc.MdiActiveDocument.CloseAndDiscard();
                    }

                    catch
                    { }
                }
                
            }
            try
            {
                //mgr_doc.MdiActiveDocument = mgr_doc.GetDocument(db);
                //mgr_doc.GetDocument(db).CloseAndDiscard();
                //mgr_doc.Open("G:\\BRATEMP\\PDS\\BLANK.DWG",false);
            }

            catch (System.Runtime.InteropServices.COMException acadcom)
            {
                Console.WriteLine("Exception was {0} type and the inner exception was {1}", acadcom.ErrorCode, acadcom.InnerException);
                Console.ReadLine();
            }
        }
    }
}
                #endregion
                #endregion