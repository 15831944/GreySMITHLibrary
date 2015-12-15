using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADLayout
    {
        public AutoCADLayout(Document parentDocument)
        {
            Document = parentDocument;
        }
        private static Document Document
        { get; set; }

        private static Layout Layout { get; set; }
        private static Database Database
        {
            get { return Document.Database; }
        }

        public string Name
        {
            get { return Layout.LayoutName; }
        }
        public List<AutoCADViewport> Viewports
        {
            get; set;
        }

        private class PaperSize : Attribute
        {
            private string Value { get; }
            public PaperSize(string value)
            {
                Value = value;
            }
        }
        private enum AutoCadLayoutSize
        {
            [PaperSize("ARCH_full_bleed_E_(36.00_x_48.00_Inches)")]
            Unknown = 0,
            [PaperSize("ANSI_full_bleed_B_(11.00_x_17.00_Inches)")]
            Tabloid = 1,
            [PaperSize("ARCH_full_bleed_B_(18.00_x_24.00_Inches)")]
            ArchB = 2,
            [PaperSize("ARCH_full_bleed_C_(17.00_x_22.00_Inches)")]
            ArchC = 3,
            [PaperSize("ARCH_full_bleed_D_(24.00_x_36.00_Inches)")]
            ArchD = 4,
            [PaperSize("ARCH_full_bleed_E_(36.00_x_48.00_Inches)"]
            ArchE = 5,
            [PaperSize("ARCH_full_bleed_E1_(30.00_x_42.00_Inches)")]
            ArchE1 = 6,
        }
    }
}
