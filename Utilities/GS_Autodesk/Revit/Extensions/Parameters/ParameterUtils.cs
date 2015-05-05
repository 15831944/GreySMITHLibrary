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
            ParameterForm paramform = ParameterForm.None;

            Debug.WriteLine("This parameter's type is: " + param.Definition.ParameterType.ToString());
            if (!param.Definition.ParameterType.ToString().Equals("Invalid"))
            {
                try
                {
                    // add something here to check if the parameter's document is already open
                    // if not - open it in a memory state.

                    using (Document doc = param.Element.Document)
                    {
                        DefinitionBindingMap dbindmap = doc.ParameterBindings;
                        Debug.WriteLine("Opening object document and seeking definition binding map");
                        Debug.WriteLine("Getting parameter definition.");
                        Definition paramDef = param.Definition as Definition;
                        Binding paramBind = null;

                        #region Testing
                        Debug.WriteLine("Getting parameter binding.");
                        DefinitionBindingMapIterator dbm_it = dbindmap.GetEnumerator() as DefinitionBindingMapIterator;

                        while (dbm_it.MoveNext())
                        {
                            Definition d = dbm_it.Key as Definition;

                            // if this isnt' the right parameter - skip to next iteration
                            if (!d.Equals(paramDef))
                                continue;
                            paramBind = dbm_it.Current as Binding;
                        }

                        #endregion


                        //var bob = dbindmap.get_Item(param.Definition);

                        if (paramBind is InstanceBinding)
                        {
                            paramform = ParameterForm.Instance;
                            Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + " is instance.");
                            Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                        }

                        else
                        {
                            if (paramBind is TypeBinding)
                            {
                                paramform = ParameterForm.Type;
                                Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + " is type.");
                                Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                            }

                            else
                            {
                                paramform = ParameterForm.None;
                                Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + "  is neither instance or type.");
                                Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                            }
                        }

                        #region old code

                        #endregion
                    }
                }

                catch (Exception e)
                {
                    Debug.WriteLine("Program ran into an exception, see info below: " + "\n"
                        + e.Source + "\n"
                        + e.StackTrace + "\n"
                        + e.Message + "\n"
                        + e.TargetSite + "\n"
                        + e.Data + "\n");
                }
            }

            return paramform;
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
                        try
                        {
                            using (Document paramdoc = param.Element.Document)
                            {
                                value = paramdoc.GetElement(id).Name;
                            }
                        }

                        catch (Autodesk.Revit.Exceptions.InvalidObjectException invalidoex)
                        {
                            Debug.WriteLine("The program failed to get the Parameter's ElementID here.");
                            Debug.WriteLine(invalidoex.Message);
                            Debug.WriteLine(invalidoex.Data);
                            Debug.WriteLine(invalidoex.StackTrace);
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
