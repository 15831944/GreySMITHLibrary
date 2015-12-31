using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADLayout : Layout, IAutoCADObject
    {
        #region Enums

        private enum PlotDevice
        {
        }

        #endregion Enums

        #region Properties

        public Document Document
        {
            get; set; }

        public IEnumerable<BlockTableRecord> ExternalReferences
        {
            get { return AutoCADUtilities.RetrieveExternalReferences(this); }
        }

        public double Height
        {
            get { return Math.Round((Layout.PlotPaperSize.Y) / 25.4); }
        }

        public LayoutType LayoutType
        {
            get; set;
        }

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

        public double Width
        {
            get { return Math.Round((Layout.PlotPaperSize.X) / 25.4); }
        }

        private Layout Layout { get; set; }

        private PlotSettings PlotSettings { get; set; }

        #endregion Properties

        #region Constructors

        public AutoCADLayout(Layout layout)
        {
            Initialize(layout);
        }

        public AutoCADLayout(Document parentDocument)
        {
            Document = parentDocument;
        }

        #endregion Constructors

        #region Methods

        public void AddViewport(Viewport viewport)
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

        private void Initialize(Layout internalLayout)
        {
            Layout = internalLayout;
            LayoutType = LayoutType.PaperSpace;
        }

        #endregion Methods
    }

    public class PaperSizeAttribute : Attribute
    {
        #region Properties

        private string Value { get; set; }

        #endregion Properties

        #region Constructors

        public PaperSizeAttribute(string value)
        {
            Value = value;
        }

        #endregion Constructors
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

    public enum LayoutType
    {
        Unknown,
        ModelSpace,
        PaperSpace
    }
}