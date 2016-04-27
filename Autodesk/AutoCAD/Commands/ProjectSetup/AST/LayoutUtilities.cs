using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GreySMITH.Domain.AutoCAD.Wrappers;

namespace GreySMITH.Domain.AutoCAD
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
