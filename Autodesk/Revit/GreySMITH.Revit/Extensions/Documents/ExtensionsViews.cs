using System.Linq;
using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Commands.Extensions.Documents
{
    public static partial class ExtensionsViews
    {
        public static View3D Create3DView(this Document currentDocument)
        {
            // Filters the model for 3D Views
            FilteredElementCollector fec3DViews =
                new FilteredElementCollector(currentDocument);

            // creates an Isometric 3D View 
            return View3D.CreateIsometric(
                    currentDocument,
                    fec3DViews.OfType<View3D>().First().Id);
        }
    }
}
