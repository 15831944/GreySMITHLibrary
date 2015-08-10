using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Extensions.Documents
{
    public static class ObjectGetter
    {
        public static List<ElementId> GetAllElementIDS(this Document doc)
        {
            List<ElementId> elemids = new List<ElementId>();
            try
            {
                FilteredElementCollector fec = new FilteredElementCollector(doc);
                elemids = fec.ToElementIds().ToList<ElementId>();
                return elemids;
            }

            catch
            {
                Console.WriteLine("There was a failure in the code, not sure what it was...");
                return elemids;
            }
        }

        public static List<ElementId> GetAllElementIDS(this Document doc, BuiltInCategory binc)
        {
            List<ElementId> elemids = new List<ElementId>();
            try
            {
                FilteredElementCollector fec = new FilteredElementCollector(doc).OfCategory(binc);
                elemids = fec.ToElementIds().ToList<ElementId>();
                return elemids;
            }

            catch
            {
                Console.WriteLine("There was a failure in grabbing element ids");
                return elemids;
            }
        }

        public static List<Element> GetAllElements(this Document doc)
        {
            List<Element> elems = new List<Element>();
            try
            {
                FilteredElementCollector fec = new FilteredElementCollector(doc);
                elems = fec.ToElements().ToList<Element>();
                return elems;
            }

            catch
            {
                Console.WriteLine("There was a failure in the code, not sure what it was...");
                return elems;
            }
        }

        public static List<Element> GetAllElements(this Document doc, BuiltInCategory binc)
        {
            List<Element> elems = new List<Element>();
            try
            {
                FilteredElementCollector fec = new FilteredElementCollector(doc).OfCategory(binc);
                elems = fec.ToElements().ToList<Element>();
                return elems;
            }

            catch
            {
                Console.WriteLine("There was a failure in the code, not sure what it was...");
                return elems;
            }
        }

        public static List<Element> GetAllElements(this Document doc, object typeofitem)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> collection = collector.OfClass(typeofitem.GetType()).ToElements().ToList();
            return collection;

        }
    }
}
