using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
            using (Transaction tr_pipedraw = new Transaction(CurrentDocument, "Drawing pipe..."))
            {
                tr_pipedraw.Start();
                double amount = CalculateRoughing(c.Owner);
                PipeType suggestedPipeType = GetSuggestedPipeType(CurrentDocument, c.PipeSystemType);
                

                // find out if the connector direction will intersect with object
                switch (IntersectsWithOwner(c))
                {
                    case (true):
                        // if so draw FROM THE OPPOSITE DIRECTION, pipe system is supply.
                        CurrentDocument.Create.NewPipe(c.Origin, (), suggestedPipeType);
                        

                        // draw a pipe in space of the appropiate size (2' away from the object)
                        //            Pipe.Create()

                        // connect the pipe back up to the fixture

                        //            CurrentDocument.Create.NewPipe();
                        break;

                    case (false):
                        // otherwise, draw away from object, pipe system is not supply

                        // do stuff
                        break;
                }
            }
        }

        /// <summary>
        /// Returns a PipeType based on the PipeType most used with a specific System Type in this document
        /// </summary>
        /// <param name="currentDocument">Document to check</param>
        /// <param name="connector">Connector to base on</param>
        /// <returns></returns>
        private PipeType GetSuggestedPipeType(Document currentDocument, PipeSystemType pipeSystemType )
        {
            throw new NotImplementedException();
        }

        private double CalculateRoughing(Element element)
        {
            double roughingAmount = 12.0;

            // if the element has a host AND that host is not a level
            if (((FamilyInstance)element).Host != null &&
                ((FamilyInstance)element).Host.GetType() != typeof(Level)) { 
                return CalculateRoughingFromHost(element);}

            // returns an number that is already converted to the Document's internal Unit System
            // i.e: changes inches to millimeters or vice versa
            return UnitUtils.ConvertToInternalUnits(roughingAmount, DisplayUnitType.DUT_FRACTIONAL_INCHES);
        }

        private double CalculateRoughingFromHost(Element element)
        {
            // typical roughing amount in inches
            double roughingAmount = 12.0;

            // variable for the host width which will be returned
            double hostWidth = 0;

            // the element's host (Host is typically a Wall, Roof, Ceiling or Floor, but could be something else)
            Element hostElement = ((FamilyInstance) element).Host;

            // define the Host Type
            switch (hostElement.Category.Name)
            {
                case "ExtrusionRoof":
                    hostWidth = ((RoofBase) hostElement).FasciaDepth;
                    break;

                case "FootPrintRoof":
                    hostWidth = ((FootPrintRoof) hostElement).FasciaDepth;
                    break;

                case "Wall":
                    hostWidth = ((Wall) hostElement).Width;
                    break;

                case "Ceiling":
                    hostWidth = ((Ceiling) hostElement).ParametersMap.get_Item("Thickness").AsDouble();
                    break;

                case "Floor":
                    hostWidth = ((Floor) hostElement).ParametersMap.get_Item("Thickness").AsDouble();
                    break;

                default:
                    Logger.Warn(
                        "The element called :'{0}' has an unknown host type called '{1}'." +
                        " Create a new case for this in the method.",
                        element.Name, hostElement.Name);
                    break;
            }

            return UnitUtils.ConvertToInternalUnits(roughingAmount, DisplayUnitType.DUT_FRACTIONAL_INCHES) + hostWidth;
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
