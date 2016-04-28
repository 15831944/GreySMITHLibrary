using System;
using NUnit.Framework;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using rvtUnit.Helpers;

namespace GreySMITH.Revit.Commands.Tests
{
    [TestFixture]
    public class DrawPipeOutCommandTests
    {
        private readonly Document currentDocument = GeneralHelper.ActiveUIDocument.Document;

        [Test]
        public void PlumbingFixtureHasConnection()
        {
            
            Assert.That(PlumbingUtils.HasOpenConnector(currentDocument,
                new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .FirstElementId()));
        }

        [Test]
        public void PlumbingFixtureIsMepModel()
        {
            TaskDialog.Show("Test Dialog", "Should appear");

            FamilyInstance plumbingFixture = 
                (FamilyInstance) new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .FirstElement();

            Assert.That(plumbingFixture.GetType().IsSubclassOf(typeof(MEPModel)));

        }
    }
}
