using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace GreySMITH.Commands
{
    public class RibbonPanelBRA : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiContApp)
        {
            uiContApp.CreateRibbonTab();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication uiApplication)
        {
            return Result.Succeeded;
        }
    }
}
