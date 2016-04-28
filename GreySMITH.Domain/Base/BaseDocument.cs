using GreySMITH.Domain.Interfaces;
using GreySMITH.Revit.Commands.Wrappers;

namespace GreySMITH.Domain.Base
{
    public abstract class BaseDocument : IDocument
    {
        public double? Id { get; }
        public string Name { get; }
        public DocumentType DocumentType { get; }
    }
}
