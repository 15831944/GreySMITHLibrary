using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Exception = System.Exception;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADLayout : Layout, IAutoCADObject
    {
        public AutoCADLayout(Layout layout)
        {
            Initialize(layout);
        }
        public AutoCADLayout(Document parentDocument)
        {
            Document = parentDocument;
        }

        public Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
            set { throw new NotImplementedException(); }
        }

        private Layout Layout { get; set; }
        public double Height
        {
            get { return Math.Round((Layout.PlotPaperSize.Y)/25.4); }
        }
        public double Width
        {
            get { return Math.Round((Layout.PlotPaperSize.X) / 25.4); }
        }
        private PlotSettings PlotSettings { get; set; }
        public string Name
        {
            get { return Layout.LayoutName; }
        }

        public LayoutSize PageSize
        {
            get { return LayoutSize.Unknown; }
            set
            {
                using (DocumentLock documentLock = Document.LockDocument())
                {
                    using (Transaction setpageTransaction = Document.Database.TransactionManager.StartTransaction())
                    {
                        PlotSettings.CopyFrom(Layout);
                        using (PlotSettingsValidator plotSettingsValidator = PlotSettingsValidator.Current)
                        {
                        }
                    }
                }
            }
        }
        public List<AutoCADViewport> Viewports
        {
            get; set;
        }

        public void AddNewViewport(Viewport viewport)
        {
            using (DocumentLock doclock = Document.LockDocument())
            {
                using (Transaction tr = Document.Database.TransactionManager.StartTransaction())
                {
                    Database db = Document.Database;
                    Editor ed = Document.Editor;

                    // Get the BlockTableRecord so you can add to it
                    BlockTable blt = tr.GetObject(db.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    BlockTableRecord btr_paper = tr.GetObject(blt[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite) as BlockTableRecord;

                    //change the current space to Paper
                    ed.SwitchToPaperSpace();

                    //switch current layer to viewport layer
                    global::Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "x-vport");
                    Document.LayerManagement("Lock", "x-vport", false);

                    // create a new viewport
                    AutoCADViewport acadViewport = new AutoCADViewport(viewport);
                    Viewport vp = acadViewport;

                    // add viewport to Paperspace and list within Document
                    btr_paper.AppendEntity(vp);
                    tr.AddNewlyCreatedDBObject(vp, true);
                    Viewports.Add(acadViewport);

                    //Enable the viewport
                    vp.On = true;

                    global::Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "0");

                    tr.Commit();
                }
            }
        }
        private class PaperSizeAttribute : Attribute
        {
            private string Value { get; set; }
            public PaperSizeAttribute(string value)
            {
                Value = value;
            }
        }
        public enum LayoutSize
        {
            [PaperSizeAttribute("ARCH_full_bleed_E_(36.00_x_48.00_Inches)")]
            Unknown = 0,
            [PaperSizeAttribute("ANSI_full_bleed_B_(11.00_x_17.00_Inches)")]
            Tabloid = 1,
            [PaperSizeAttribute("ARCH_full_bleed_B_(18.00_x_24.00_Inches)")]
            ArchB = 2,
            [PaperSizeAttribute("ARCH_full_bleed_C_(17.00_x_22.00_Inches)")]
            ArchC = 3,
            [PaperSizeAttribute("ARCH_full_bleed_D_(24.00_x_36.00_Inches)")]
            ArchD = 4,
            [PaperSizeAttribute("ARCH_full_bleed_E_(36.00_x_48.00_Inches)")]
            ArchE = 5,
            [PaperSizeAttribute("ARCH_full_bleed_E1_(30.00_x_42.00_Inches)")]
            ArchE1 = 6,
        }
        private enum PlotDevice
        {
            
        }

        private void Initialize(Layout internalLayout)
        {
            Layout = internalLayout;
        }
    }
}
