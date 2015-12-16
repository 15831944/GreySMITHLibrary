using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADLayout : Layout, IList<Viewport>
    {
        public AutoCADLayout(Layout layout)
        {
            Initialize(layout);
        }
        public AutoCADLayout(Document parentDocument)
        {
            Document = parentDocument;
        }
        private static Document Document
        { get; set; }
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
            get;
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

        private class PaperSizeAttribute : Attribute
        {
            private string Value { get; set; }
            public PaperSizeAttribute(string value)
            {
                Value = value;
            }
        }
        private enum LayoutSize
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
