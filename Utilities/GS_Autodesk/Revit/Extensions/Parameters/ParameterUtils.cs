using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;

using GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Applications;
using GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Documents;
using GreySMITH.Utilities.General;

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Parameters
{
    static class ParameterUtils
    {
        static bool IsInstance(this Parameter param)
        {
            bool i = false;
            Document doc = param.Element.Document;

            BindingMap bm = doc.ParameterBindings;
            DefinitionBindingMapIterator dbmi = bm.ForwardIterator();
            int bindingmapsize = bm.Size;
            Debug.Print("testing");
            
            
            while(dbmi.MoveNext())
            {
                Definition def = dbmi.Key as Definition;
                Binding bin = dbmi.Current as Binding;
            }
            
            
            /* Parameter is a concrete class
             * of the abstract class DEFINITION
             * all of the info we want about the parameter comes from
             * the definition of the parameter
             */
            Definition d = param.Definition;
            // name
            string param_name = d.Name;

            // kind of parameter (i.e.: Yes|No, Volume, Horsepower
            ParameterType param_kind = d.ParameterType;

            // 
            
            
            

            //Element e = param.Element;

            //BindingMap 

            return i;
        }


    }
}
