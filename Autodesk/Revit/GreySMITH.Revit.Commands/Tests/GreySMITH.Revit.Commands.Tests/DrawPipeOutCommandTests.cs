using System;
using NUnit.Framework;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using rvtUnit.Helpers;

namespace GreySMITH.Revit.Commands.Tests
{
    [TestFixture]
    public class DrawPipeOutCommandTests
    {
        [SetUp]
        public void Setup()
        {
            

        }

        [Test]
        public void PlumbingFixtureHasConnection()
        {
            Document currentDocument = GeneralHelper.ActiveUIDocument.Document;
            Assert.That(PlumbingUtils.HasOpenConnector(currentDocument,
                new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .FirstElementId()));
        }

        [Test]
        public void PlumbingFixtureIsMepModel()
        {
            Document currentDocument = GeneralHelper.ActiveUIDocument.Document;

            FamilyInstance plumbingFixture = 
                (FamilyInstance) new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .FirstElement();

            Assert.That(plumbingFixture.GetType().IsSubclassOf(typeof(MEPModel)));

        }
    }
}
