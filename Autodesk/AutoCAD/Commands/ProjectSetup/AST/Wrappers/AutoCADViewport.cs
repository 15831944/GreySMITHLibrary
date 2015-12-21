using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    /// <summary>
    /// A class which holds all the data necessary to recreate a viewport
    /// </summary>
    public class AutoCADViewport : Viewport, IAutoCADObject
    {
        private Viewport Viewport { get; set; }
        public Document Document
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public AutoCADViewport(Viewport viewport)
        {
            Initialize(viewport);
        }
        public IEnumerable<ObjectId> FrozenLayers 
        {
            get
            {
                return (from ObjectId x in Viewport.GetFrozenLayers()
                        where !x.IsNull
                        select x);
            }
        }
        private void Initialize(Viewport internalViewport)
        {
            Viewport = internalViewport;
        }
    }
}
