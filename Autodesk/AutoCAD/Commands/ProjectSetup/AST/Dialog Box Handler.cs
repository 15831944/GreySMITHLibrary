using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Windows;
using Autodesk.AutoCAD.Windows;
using ExtensionMethods;

//change this to make the appropiate project name
namespace Project_Name
{
    // name of class below
    public class Class1
    {
        //public void MethodCode
        //{
        
        //}

        // delete this after putting in the command name below
        // and deciding whether the command applies across all session
        [CommandMethod("CommandName", CommandFlags.Session)]
        public static void Main_Testing_Area()
        {
            
        }
    }
}
