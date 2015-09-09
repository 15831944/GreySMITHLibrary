using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GreySMITH.Revit.Wrappers;
using GreySMITH.Revit.Extensions.Elements;

namespace GreySMITH.Revit.Wrappers
{
    public interface IRevitObject
    {
        Document CurrentDocument { get; }
    }
}
