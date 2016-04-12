using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.Wrappers
{
    public abstract class RevitObject : IRevitObject
    {
        public Document SourceDocument{get;}

        protected RevitObject(Document currentDocument)
        {
            SourceDocument = currentDocument;
        }
    }

    public interface IRevitObject
    {
        Document SourceDocument { get; }
    }
}
