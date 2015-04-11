using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;

using GreySMITH.Utilities.General;

namespace GreySMITH.Utilities.Autodesk.Revit.Automation
{
    public static class SendRevitCommand
    {
        public static void CopyMonitorLinkedObjects()
        {
            try
            {
                SendKeys.SendWait(RibbonCommandShortcuts.COLLABORATE_COPY_MONITOR_SELECT_LINK.GetStringValue());
            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace + e.Message);
                throw e;
            }
        }
    }
}
