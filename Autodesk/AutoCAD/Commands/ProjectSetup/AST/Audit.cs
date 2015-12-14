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

namespace Cleaners
{
    public static class Audit
    {
        public static void NET_AUDIT (this Document doc)
        {
            try
            {
                doc.SendStringToExecute("_AUDIT\n", true, false, true);
                doc.SendStringToExecute("Y\n", true, false, true);
            }

            catch
            {
            }
        }

        public static void NET_PURGE(this Document doc)
        {
            try
            {
                doc.SendStringToExecute("-PURGE\n", true, false, true);
                doc.SendStringToExecute("All\n", true, false, true);
                doc.SendStringToExecute("*\n", true, false, true);
                doc.SendStringToExecute("N\n", true, false, true);
            }

            catch
            {
            }
        }

        public static void NET_RECOVER(this Document doc)
        {
            try
            {
                doc.SendStringToExecute("RECOVER\n", true, false, true);
                doc.SendStringToExecute((doc.Name + "\n"), true, false, false);
            }

            catch
            {
            }
        }
    }

    public static class Test
    {
        [CommandMethod("NETAUD", CommandFlags.Session)]
        public static void Main()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.NET_AUDIT();
            doc.NET_PURGE();
            doc.NET_RECOVER();
        }
    }
}
