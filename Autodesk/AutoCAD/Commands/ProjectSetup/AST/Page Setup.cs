using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ExtensionMethods;

namespace PageSetup
{
    public static class PaperSetupMethods
    {
        public static void SetPageSize (this Layout curlay, double width, double height, PlotPaperUnit ppu)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            LayoutManager laymgr = LayoutManager.Current;

            using (DocumentLock dl = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    using (PlotSettings pltset = new PlotSettings(curlay.ModelType))
                    {
                        pltset.CopyFrom(curlay);
                        PlotSettingsValidator psv = PlotSettingsValidator.Current;
                        Layout layout = new Layout();

                        try
                        {
                            if (curlay.ObjectId != null)
                            {
                                layout = tr.GetObject(curlay.ObjectId, OpenMode.ForWrite) as Layout;
                            }
                        }

                        catch
                        {
                        }

                        #region Paper Types
                        try
                        {
                            if (!ppu.Equals(PlotPaperUnit.Inches))
                            {
                                psv.SetPlotPaperUnits(pltset, PlotPaperUnit.Inches);
                            }

                            if ((width == 11.00 && height == 17.00) || (width == 17.00 && height == 11.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ANSI_full_bleed_B_(11.00_x_17.00_Inches)");
                            }

                            if ((width == 18.00 && height == 24.00) || (width == 24.00 && height == 18.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_B_(18.00_x_24.00_Inches)");
                            }

                            else if ((width == 17.00 && height == 22.00) || (width == 22.00 && height == 17.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_C_(17.00_x_22.00_Inches)");
                            }

                            else if ((width == 24.00 && height == 36.00) || (width == 36.00 && height == 24.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_D_(24.00_x_36.00_Inches)");
                            }

                            else if ((width == 36.00 && height == 48.00) || (width == 48.00 && height == 36.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_E_(36.00_x_48.00_Inches)");
                            }

                            else if ((width == 30.00 && height == 42.00) || (width == 42.00 && height == 30.00))
                            {
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_E1_(30.00_x_42.00_Inches)");
                            }

                            else
                            {
                                doc.Editor.WriteMessage("\n The drawing sizes were invalid, default setting will be used");
                                psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_E_(36.00_x_48.00_Inches)");
                            }
                        }

                        catch
                        {
                            doc.Editor.WriteMessage("\n The drawing sizes were invalid, default setting will be used");
                            psv.SetPlotConfigurationName(pltset,
                                "DWG To PDF.pc3",
                                "ARCH_full_bleed_E_(36.00_x_48.00_Inches)");
                        }
                        #endregion

                        psv.SetZoomToPaperOnUpdate(pltset, true);

                        layout.UpgradeOpen();
                        layout.CopyFrom(pltset);

                        doc.Editor.WriteMessage("\n  " + "\nNew device name: " + layout.PlotConfigurationName);

                        tr.Commit();
                    }
                }
            }
        }

        // Testing area for the actual command
        [CommandMethod("PGSET", CommandFlags.Session)]
        public static void MainTesting()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            LayoutManager laymgr = LayoutManager.Current;
            try
            {
                Layout lay = doc.Create_NewLayout("Testing");

                double w = 25.00;
                double h = 30.00;
                PlotPaperUnit ppunits = PlotPaperUnit.Inches;

                lay.SetPageSize(w, h, ppunits);
            }

            catch
            {
            }
        }
    }
}
