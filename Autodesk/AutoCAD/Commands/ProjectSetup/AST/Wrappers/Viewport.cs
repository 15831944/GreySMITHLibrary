using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    /// <summary>
    /// A class which holds all the data necessary to recreate a viewport
    /// </summary>
    public class Viewport
    {
        //creates the specific fields for this class
        public StandardScaleType StandardScale;
        public double CustomScale;
        public Point3d CenterPoint;
        public Point2d ViewCenter;
        public double Width;
        public double Height;
        public double ViewHeight;
        public Point3d ViewTarget;
        public IEnumerable<ObjectId> FrozenLayers;

        public Viewport(
            StandardScaleType Standard_Scale,
            double Custom_Scale,
            Point3d Center_Point,
            Point2d View_Center,
            double width,
            double height,
            double View_Height,
            Point3d View_Target,
            ObjectIdCollection Frozen_Layers)
        {
            //assigns the parameters added to the fields for this class
            StandardScale = Standard_Scale;
            CustomScale = Custom_Scale;
            CenterPoint = Center_Point;
            ViewCenter = View_Center;
            Width = width;
            Height = height;
            ViewHeight = View_Height;
            ViewTarget = View_Target;
            FrozenLayers = from ObjectId x in Frozen_Layers
                where !x.IsNull
                select x;
        }
    }
}
