using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Threading;
using System.Timers;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ExtensionMethods;

namespace EventHandlers_ACAD
{
    public class MyTimerClass
    {
        public event EventHandler Elapsed;

        private void OnOneSecond(object source, EventArgs args)
        {
            if (Elapsed != null)
            {
                Elapsed(source, args);
            }
        }
    }

    public class MyTCEventArgs : EventArgs
    {
        public string message;
        public MyTCEventArgs(string s)
        {
            message = s;
        }
    }

    //Custom Delegate Examples
    public delegate void MyTCEventHandler (object sender, MyTCEventArgs e);

    //or 

    public class ExampleClass
    {
        public event EventHandler<MyTCEventArgs> Elapsed;

        public void Main()
        {
        }
    }

    public class SecondaryClass
    {
        MyTimerClass mc;
    }

    public class Class1
    {
        [CommandMethod("EVNT", CommandFlags.Session)]
        public static void Main_Testing_Area()
        {
            
        }
    }
}
