using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Wrappers
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
