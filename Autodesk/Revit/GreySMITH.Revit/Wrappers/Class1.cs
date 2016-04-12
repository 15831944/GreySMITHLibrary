using System;
using System.Collections.Generic;
using System.Text;

namespace GreySMITH.Revit.Wrappers
{
    public abstract class BaseDocument : IDocument
    {
        public double? Id { get; }
        public string Name { get; }
        public DocumentType DocumentType { get; }
    }

    public interface IDocument
    {
        double? Id { get; }
        string Name { get; }
        DocumentType DocumentType { get; }

    }

    public enum DocumentType
    {
        Unknown = -1,
        None = 0,
        Revit = 1,
        AutoCAD = 2,
        Navisworks = 3
    }
}
