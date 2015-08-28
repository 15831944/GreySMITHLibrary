using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Wrappers
{
    public abstract partial class RevitExtension : IRevitExtension
    {
        public abstract Document CurrentDocument { get; }
    }
}
