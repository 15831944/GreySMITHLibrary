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
            bool instancetruth = false;
            using (Document doc = param.Element.Document)
            {
                DefinitionBindingMap dbindmap = doc.ParameterBindings as DefinitionBindingMap;
                Definition paramDef = param.Definition;
                Binding binding = dbindmap.get_Item(paramDef);

                if (binding is InstanceBinding)
                {
                    instancetruth = true;
                }
            }

            return instancetruth;
        }
    }

    ///* Parameter is a concrete class
    // * of the abstract class DEFINITION
    // * all of the info we want about the parameter comes from
    // * the definition of the parameter
    // */
    //Definition d = param.Definition;
    //// name
    //string param_name = d.Name;

    //// kind of parameter (i.e.: Yes|No, Volume, Horsepower
    //ParameterType param_kind = d.ParameterType;
}
