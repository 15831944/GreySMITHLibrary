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


namespace Cleaners
{
    [CommandMethod("NETAUD", CommandFlags.Session)]
    public class Clean
    {
        public static void Net_Audit(this Document doc)
        {
            doc.SendStringToExecute("_AUDIT\n", true, false, true);
            //doc.SendStringToExecute(
        }
    }
}
