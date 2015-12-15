using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.Wrappers
{
    //TODO: Do you still need to create a RevitObject and IRevitObject?
    public partial class RevitObject : IRevitObject
    {
        public readonly Document _document;

        public Document CurrentDocument
        {
            get
            {
                return _document;
            }
        }

        public RevitObject(Document currentDocument)
        {
            _document = currentDocument;
        }
    }
}
