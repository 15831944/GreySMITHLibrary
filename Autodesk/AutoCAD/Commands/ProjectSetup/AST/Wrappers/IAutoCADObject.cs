using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public interface IAutoCADObject
    {
        Document Document { get; set; }
        Database Database { get; }
    }
    public abstract class AutoCADObject : IAutoCADObject
    {
        public Document Document { get; set; }
        public Database Database
        {
            get { return Document.Database; }
        }
    }
}
