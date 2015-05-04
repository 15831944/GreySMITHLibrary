using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    public static class ParameterUtils
    {
        public static FileStream log_instparam = new FileStream(@"C:\temp\Revit\InstanceParameterSync\debuglog.log", FileMode.OpenOrCreate);
        public static TextWriterTraceListener ip_logger = new TextWriterTraceListener(log_instparam);

        public static ParameterType GetParameterType(this Parameter param)
        {
            Debug.Listeners.Add(ip_logger);
            Debug.Indent();
            Debug.AutoFlush = true;

            ParameterType paramtype = ParameterType.None;

            try
            {
                
                using (Document doc = param.Element.Document)
                {
                    using (DefinitionBindingMap dbindmap = doc.ParameterBindings as DefinitionBindingMap)
                    {
                        Debug.WriteLine("Opening object document and seeking definition binding map");
                        Definition paramDef = param.Definition;
                        Debug.WriteLine("Getting parameter definition.");
                        Binding binding = dbindmap.get_Item(paramDef);
                        Debug.WriteLine("Getting parameter binding.");

                        if (binding is InstanceBinding)
                        {
                            paramtype = ParameterType.Instance;
                        }

                        else
                        {
                            if(binding is TypeBinding)
                            {
                                paramtype = ParameterType.Type;
                            }

                            else
                            {
                                paramtype = ParameterType.None;
                            }
                        }
                    }
                }
            }

            catch(Exception e)
            {

            }

            return paramtype;
        }

        public enum ParameterType
        {
            [StringValueAttribute("Instance")]
            Instance = 1,
            [StringValueAttribute("Type")]
            Type = 2,
            [StringValueAttribute("None")]
            None = 3
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
