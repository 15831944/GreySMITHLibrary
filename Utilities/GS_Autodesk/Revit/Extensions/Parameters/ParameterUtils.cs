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
        public static ParameterForm GetParameterForm(this Parameter param)
        {
            ParameterForm paramtype = ParameterForm.None;
            
            using (FileStream log_instparam = new FileStream(@"C:\temp\Revit\InstanceParameterSync\debuglog2.log", FileMode.OpenOrCreate))
            {
                try
                {
                    TextWriterTraceListener ip_logger = new TextWriterTraceListener(log_instparam);
                    Debug.Listeners.Add(ip_logger);
                    Debug.Indent();
                    Debug.AutoFlush = true;

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
                                paramtype = ParameterForm.Instance;
                            }

                            else
                            {
                                if (binding is TypeBinding)
                                {
                                    paramtype = ParameterForm.Type;
                                }

                                else
                                {
                                    paramtype = ParameterForm.None;
                                }
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    Debug.WriteLine("Program failed: " + e.StackTrace);
                }
            }


            return paramtype;
        }

        public static string GetParameterValue(this Parameter param)
        {
            string value = null;

            switch(param.StorageType)
            {
                default:
                    // A NULL VALUE MEANS THE PARAMETER IS UNEXPOSED
                    value = "PARAMETER HAS NOT BEEN EXPOSED";
                    break;

                case StorageType.Double:
                    value = "DOUBLE: " + param.AsDouble().ToString();
                    break;

                case StorageType.Integer:
                    if (ParameterType.YesNo == param.Definition.ParameterType)
                    {
                        if (param.AsInteger() == 0)
                        {
                            value = "False";
                        }
                        else
                        {
                            value = "True";
                        }
                    }
                    else
                    {
                        value = "INTEGER: " + param.AsInteger().ToString();
                    }
                    break;

                case StorageType.String:
                    value = param.AsString();
                    break;

                case StorageType.ElementId:
                    // this one is tricky
                    // a positive ElementID can point to a specific element
                    // however a negative one can mean a number of different things
                    ElementId id = param.AsElementId();

                    if (id.IntegerValue >= 0)
                    {
                        using (Document paramdoc = param.Element.Document)
                        {
                            value = paramdoc.GetElement(id).Name;
                        }
                    }

                    else
                    {
                        value = "ELEMENTID: " + id.IntegerValue.ToString();
                    }
                    break;

            }

            return value;
        }

        public enum ParameterForm
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
