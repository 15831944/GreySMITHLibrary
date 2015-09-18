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
using GreySMITH.Revit.Extensions.Documents;

namespace GreySMITH.Commands.DrawPipeOuts
{
    /// <summary>
    /// Command designed to allow the user to "rough out" the piping for multiple plumbing fixtures simultaneously
    /// </summary>
    public partial class DrawPipeOutCommand : AbstractCommand
    {
        // default constructor for concrete classes - make sure to implement this in all cases.
        public DrawPipeOutCommand(
            ExternalCommandData excmd,
            string mainmessage,
            Autodesk.Revit.DB.ElementSet elemset)
            : base(excmd, mainmessage, elemset)
        {
            _mainMessage = mainmessage;
            _externalCMD = excmd;
            _elementSet = elemset;
        }

        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();
        private List<ElementId> ListOfElementsWithoutConnectors =
            new List<ElementId>();
        private Dictionary<Connector, Element> _connectorDictionary =
            new Dictionary<Connector, Element>();

        public Dictionary<Connector, Element> ConnectorDictionary
        {
            get
            {
                return _connectorDictionary;
            }
        }
        private void CreateConnectorDictionary()
        {
            IEnumerable<Connector> fecConnectors =
                new FilteredElementCollector(CurrentDocument).OfType<Connector>();

            foreach (Connector connector in fecConnectors)
            {
                _connectorDictionary.Add(connector, connector.Owner);
            }
        }
        public override Result Work()
        {
            // create a dictionary of elements in the model vs connectors
            CreateConnectorDictionary();

            // select the object(s)
            IList<Element> plumbingFixtures =
                PromptUserToSelectObjects();

            // draw a pipe from the object's connection of the correct system type in the appropiate direction
            DrawRoughing(plumbingFixtures);
            
            return Result.Failed;
        }
        private IList<Element> PromptUserToSelectObjects()
        {
            IList<Element> selectedElements = new List<Element>();
            using (Selection selected = UiDocument.Selection)
            {
                IEnumerable<Reference> refObjects = selected.PickObjects(ObjectType.Element);
                
                // for some reason this doesn't work with ElementIds?
                //IEnumerable<ElementId> selectedElementIds =
                //    refObjects.SelectMany<Reference, ElementId>(x => x.ElementId);

                foreach(Reference r in refObjects)
                {
                    ElementId referenceId = r.ElementId;
                    selectedElements.Add(CurrentDocument.GetElement(referenceId));
                }
            }

            return selectedElements;
        }
        private bool HasConnection(Element element)
        {
            if (_connectorDictionary.ContainsValue(element))
                return true;
            return false;
        }
        private void DrawRoughing(IList<Element> elementList)
        {
            foreach (Element element in elementList)
            {
                DrawPipeToOrFromObject(element);
            }

            TaskDialog.Show(
                "Roughing Results",
                string.Format(
                    "{0} objects were unable to be drawn because of a lack of connectors. " +
                    "Speak to the architect and creator of the family so" +
                    "that a connector can be added to the family", 
                    ListOfElementsWithoutConnectors.Count()));
        }
        private void DrawPipeToOrFromObject(Element element)
        {
            // find the object
            XYZ elementLocation =       
                ((LocationPoint) element.Location).Point;

            // if none - keep track and let the user know to add a connector for that fixture
            if (!HasConnection(element))
            {
                Logger.Debug("ElementID {0} contained no connections and was neither drawn to nor from.", element.Id);
                ListOfElementsWithoutConnectors.Add(element.Id);
                return;
            }
            
            foreach (Connector c in (ConnectorDictionary.Keys.Where(c => c.Owner == element)))
            {
                DrawPipeToOrFromConnector(c);
            }
        }
        private void DrawPipeToOrFromConnector(Connector c)
        {
            double amount = CalculateRoughing(c.Owner);

            // find out if the connector direction will intersect with object
            switch (IntersectsWithOwner(c))
            {
                // if so draw FROM THE OPPOSITE DIRECTION, pipe system is supply.


                // otherwise, draw away from object, pipe system is not supply
                // draw a pipe in space of the appropiate size (2' away from the object)
                //            Pipe.Create()


                // connect the pipe back up to the fixture

                //            CurrentDocument.Create.NewPipe();   

                case (true):
                    // do stuff
                    break;

                case (false):
                    // do stuff
                    break;
            }
        }
        // TODO figure out how to calculate roughing
        private double CalculateRoughing(Element element)
        {
            Units documentUnits = element.Document.GetUnits();
            DisplayUnit documentUnitSystem = element.Document.DisplayUnitSystem;
            
            
            double roughingAmount = 0.0;

            // if the element has a host 
            if (((FamilyInstance)element).Host != null) { 
                return CalculateRoughingFromHost(element);}

            return roughingAmount;
        }

        private double CalculateRoughingFromHost(Element element)
        {
            double roughingAmount = 12.0;
            double hostWidth;

            Element hostElement = ((FamilyInstance) element).Host;
            var hostElementWidth = hostElement.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
            double.TryParse(hostElementWidth.AsValueString(), out hostWidth);
            // do stuff

            return roughingAmount;
        }

        private void ConvertToDocumentUnits(double value)
        {
            
        }

        private bool IntersectsWithOwner(Connector c)
        {
            ReferenceWithContext intersectedReference = 
                new ReferenceIntersector(                           // shoots a ray from the connector's face
                c.Owner.Id,                                         // element it looks for intersection with
                FindReferenceTarget.All,                            // look for all intersections because this should hit it's owner first anyway if they intersect
                CurrentDocument.Create3DView()).                    // newly created 3DView
                FindNearest(c.Origin, c.CoordinateSystem.BasisZ);   // gives the normal from the connector's face

            // if the object returned is not null AND is the element itself, the connector intersects with it
            if (null != intersectedReference.GetReference() &&
                intersectedReference.GetReference() == new Reference(c.Owner)) { 
                return true;}

            return false;
        }

        #region Un-Ready Methods
        /// <summary>
        /// This might have to wait - realizing that it would be quite a task
        /// to programmatically place the connector because the user would
        /// expect it come out of very specific places for each piece of geometry
        /// </summary>
        /// <param name="instanceOfObject">object to add connector to</param>
        private void AddConnector(FamilyInstance instanceOfObject)
        {
            Document familyDocument = instanceOfObject.Symbol.Document;

            using (Transaction tr_AddConnector = new Transaction(familyDocument))
            {
                tr_AddConnector.Start("Adding connector to the current family");



            }
        }
        #endregion
    }
}
