using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.ApplicationParts;
using Autodesk.Navisworks.Api.Automation;
using Autodesk.Navisworks.Api.Controls;
using Autodesk.Navisworks.Api.Data;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreySMITH.Utilities.GS_Autodesk.Navisworks
{
    [PluginAttribute("BasicPlugIn.ABasicPlugin",                   //Plugin name
                     "ADSK",                                       //4 character Developer ID or GUID
                     ToolTip = "BasicPlugIn.ABasicPlugin tool tip",//The tooltip for the item in the ribbon
                     DisplayName = "Hello World Plugin")]          //Display name for the Plugin in the Ribbon


    class TestNavisClass : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            //throw new NotImplementedException();

            //starts an invisible instance of Navisworks
            NavisworksApplication app_nav = new NavisworksApplication();
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            foreach(string s in parameters)
            {
                // adds a to the current navisworks document
                app_nav.AppendFile(s);
            }

            // save Navisworks file in location
            app_nav.SaveFile(@"C:\test\newnavisfile.nwd");

            // program failed
            return -1;
        }
    }
}
