using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Runtime.InteropServices;
using System.Windows.Forms;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

using GreySMITH.Utilities.General;

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Automation
{
    public static class SendRevitCommand
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #region not useful
        //[DllImport("USER32.DLL")]
        //static const string _window_class_name_zero
        //    = "Afx:0000000140000000:8:0000000000010005:0000000000000000:FFFFFFFFAEF91013";

        //[DllImport("USER32.DLL")]
        //static const string _window_class_name_open
        //    = "Afx:0000000140000000:8:0000000000010005:0000000000000000:FFFFFFFFAEF91013";
        #endregion

        public static void CopyMonitorLinkedObjects(Document doc)
        {
            try
            {
                //SendKeys.SendWait(RibbonCommandShortcuts.COLLABORATE_COPY_MONITOR_SELECT_LINK.GetStringValue());

                //SendKeys.SendWait("{F10}");
                //SendKeys.SendWait("{C}");
                //SendKeys.SendWait("{CM}");
                //SendKeys.SendWait("{DOWN}");
                //SendKeys.SendWait("{DOWN}");
                //SendKeys.SendWait("{ENTER}");

                //SHOULD CAUSE HELP TO POP OUT
                IntPtr revithandle = 
                    System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                bool worked = SetForegroundWindow(revithandle);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(HelpMe));
                //SendKeys.SendWait(RibbonCommandShortcuts.COLLABORATE_COPY_MONITOR_SELECT_LINK.GetStringValue());
                //SendKeys.SendWait("{XC}");

                // fails because Windows believes the application running this project is the foreground window
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        static void HelpMe(object stateInfo)
        {
            try
            {
                SendKeys.SendWait("{F1}");
            }

            catch( Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
    }
}
