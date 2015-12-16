using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using GreySMITH.Autodesk.AutoCAD.Wrappers;
using Exception = System.Exception;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class LayoutUtilities
    {
        private static Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        public static void SetPageSize(Layout layout)
        {
            using (DocumentLock documentLock = Document.LockDocument())
            {
                using (Transaction setpageTransaction = Document.Database.TransactionManager.StartTransaction())
                {
                    using (PlotSettings plotSettings = new PlotSettings(layout.ModelType))
                    {
                        using (PlotSettingsValidator plotSettingsValidator = PlotSettingsValidator.Current)
                        {
                            layout = (Layout) setpageTransaction.GetObject(layout.ObjectId, OpenMode.ForWrite);

                            plotSettingsValidator.SetZoomToPaperOnUpdate(plotSettings, true);
                            plotSettingsValidator.SetPlotPaperUnits(plotSettings, PlotPaperUnit.Inches);
                            
                            

                        }
                    }
                }
            }
        }

        private static void CalculatePlotArea(Layout layout)
        {
            AutoCADLayout acadLayout = layout as AutoCADLayout;
            
        }
    }
}
