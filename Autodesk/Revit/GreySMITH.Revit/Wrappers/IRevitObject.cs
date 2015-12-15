using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.Wrappers
{
    public interface IRevitObject
    {
        Document CurrentDocument { get; }
    }
}
