using GreySMITH.Revit.Commands.Wrappers;

namespace GreySMITH.Domain.Interfaces
{
    public interface IDocument
    {
        double? Id { get; }
        string Name { get; }
        DocumentType DocumentType { get; }

    }
}