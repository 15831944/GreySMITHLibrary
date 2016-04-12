using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using GreySMITH.Common.General;
using NLog;

namespace GreySMITH.Revit.Commands.Extensions.Parameters
{
    /// <summary>
    // Parameter is a concrete class
    // of the abstract class DEFINITION
    // all of the info we want about the parameter comes from
    // the definition of the parameter
    // 
    // Definition d = param.Definition;
    // name
    // string param_name = d.Name;
    // kind of parameter (i.e.: Yes|No, Volume, Horsepower
    //ParameterType param_kind = d.ParameterType;
    /// </summary>
    public static class ParameterUtils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static ParameterForm GetParameterForm(this Parameter param)
        {
            ParameterForm paramform = ParameterForm.None;
            try
            {
                if (param.Element.Document.IsFamilyDocument)
                {
                    Debug.Print("Element isn't useable in this operation.");
                    return ParameterForm.None;
                }

                #region Failed Code
                    //if (!param.Definition.ParameterType.ToString().Equals("Invalid"))
                    //{
                    //    try
                    //    {
                    //        // add something here to check if the parameter's document is already open
                    //        // if not - open it in a memory state.

                    //        using (Document doc = param.Element.Document)
                    //        {
                    //            // will not work with Family Documents
                    //            Debug.Assert(doc.IsFamilyDocument, "Document is a family - this parameter will be skipped.");

                    //            // binding map has only one entry?
                    //            DefinitionBindingMap dbindmap = doc.ParameterBindings;

                    //            Debug.WriteLine("Opening object document and seeking definition binding map");
                    //            Debug.WriteLine("Getting parameter definition.");
                    //            Definition paramDef = param.Definition as Definition;
                    //            Binding paramBind = null;

                    //            #region Testing
                    //            Debug.WriteLine("Getting parameter binding.");
                    //            DefinitionBindingMapIterator dbm_it = dbindmap.GetEnumerator() as DefinitionBindingMapIterator;

                    //            while (dbm_it.MoveNext())
                    //            {
                    //                // definition found in bindmapiterator is completely different than the used parameter?
                    //                Definition d = dbm_it.Key as Definition;

                    //                // if this isnt' the right parameter - skip to next iteration
                    //                if (!d.Equals(paramDef))
                    //                    continue;
                    //                paramBind = dbm_it.Current as Binding;
                    //            }

                    //            #endregion

                    //            //var bob = dbindmap.get_Item(param.Definition);

                    //            if (paramBind is InstanceBinding)
                    //            {
                    //                paramform = ParameterForm.Instance;
                    //                Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + " is instance.");
                    //                Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                    //            }

                    //            else
                    //            {
                    //                if (paramBind is TypeBinding)
                    //                {
                    //                    paramform = ParameterForm.Type;
                    //                    Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + " is type.");
                    //                    Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                    //                }

                    //                else
                    //                {
                    //                    paramform = ParameterForm.None;
                    //                    Debug.WriteLine("Parameter: " + param.Definition.Name.ToString() + "  is neither instance or type.");
                    //                    Debug.WriteLine("Parameter value is: " + param.GetParameterValue());
                    //                }
                    //            }
                    //        }
                    //    }

                    //    catch (Exception e)
                    //    {
                    //        Debug.WriteLine("Program ran into an exception, see info below: " + "\n"
                    //            + e.Source + "\n"
                    //            + e.StackTrace + "\n"
                    //            + e.Message + "\n"
                    //            + e.TargetSite + "\n"
                    //            + e.Data + "\n");
                    //    }
                    //}
                    #endregion

                // supposedly FamilyInstances hold all the instance parameters and FamilySymbols hold all the 
                // the type parameters
                // will not work with System Families
                if (param.Element is FamilyInstance)
                {
                    Debug.Print("'" + param.Definition.Name.ToString() + "'" + " is an instance parameter.");
                    return ParameterForm.Instance;
                }

                if (param.Element is FamilySymbol)
                {
                    Debug.Print("'" + param.Definition.Name.ToString() + "'" + " is an type parameter.");
                    return ParameterForm.Type;
                }

                // need code to deal with System Families like Walls,
                // Ducts, Pipe
                // HostObject seems to be the one that's most consistent
                // will capture the above + floors, cable tray etc...
                //if (param.Element is HostObject)
                //{
                Debug.Print("The " + param.Element.Name + " is a HostObject, not a FamilyInstance or FamilySymbol");

                // still need to find a way to distinguish elements which are from HostObject elements.
                return ParameterForm.Invalid;
            }

            catch (Exception e)
            {
                Debug.WriteLine("Program ran into an exception, see info below: " + "\n"
                    + e.Source + "\n"
                    + e.StackTrace + "\n"
                    + e.Message + "\n"
                    + e.TargetSite + "\n"
                    + e.Data + "\n");
                return ParameterForm.Invalid;
            }
        }

        //public static ParameterForm GetParameterForm(this Parameter param, Document doc)
        //{
        //    ParameterForm pf = ParameterForm.None;

        //    Debug.WriteLine("This parameter's type is: " + param.Definition.ParameterType.ToString());
        //    if (!param.Definition.ParameterType.ToString().Equals("Invalid"))
        //    {
        //        try
        //        {
        //            DefinitionBindingMap dbm = doc.ParameterBindings;
        //            Debug.WriteLine("The DefinitionBindingMap has " + dbm.Size.ToString() + "entries");

        //            Definition d = param.Definition;
        //            Binding b = dbm.get_Item(d);

        //            Debug.Assert(b is InstanceBinding, (d.Name + " is an instance"));
        //            Debug.Assert(b is TypeBinding, (d.Name + " is a type"));
        //        }

        //        catch (Autodesk.Revit.Exceptions.InvalidObjectException invOe)
        //        {
        //            // coode
        //            Debug.WriteLine("The object was invalid?" + "\n" + invOe.Message);
        //        }

        //        catch (Autodesk.Revit.Exceptions.InvalidOperationException auto_ioe)
        //        {
        //            // code
        //            Debug.WriteLine("The operation was invalid?" + "\n" + auto_ioe.Message);
        //        }
        //    }

        //    return pf;
        //}

        public static string GetParameterValue(this Parameter param)
        {
            try
            {
                switch (param.StorageType)
                {
                    default:
                        // A NULL VALUE MEANS THE PARAMETER IS UNEXPOSED
                        return "PARAMETER HAS NOT BEEN EXPOSED";

                    case StorageType.Double:
                        return GetParameterDouble(param);

                    case StorageType.Integer:
                        if (ParameterType.YesNo != param.Definition.ParameterType)
                            return "INTEGER: " + param.AsInteger().ToString();

                        if (param.AsInteger() == 0)
                            return "False";

                        return "True";

                    case StorageType.String:
                        return param.AsString();

                    case StorageType.ElementId:
                        // this one is tricky
                        // a positive ElementID can point to a specific element
                        // however a negative one can mean a number of different things

                        if (param.AsElementId().IntegerValue <= 0)
                            return "ELEMENTID: " + param.AsElementId().IntegerValue.ToString();

                        using (Document paramdoc = param.Element.Document)
                        {
                            return paramdoc.GetElement(param.AsElementId()).Name;
                        }
                }
            }


            catch (InvalidObjectException invalidoex)
            {
                logger.Debug("There was an exception: {0}", invalidoex);

            }

            return null;
        }

        public static string GetParameterDouble(Parameter param)
        {
            return "DOUBLE: " + param.AsDouble().ToString();
        }

        public static void SetParameterValue(this Parameter param)
        {
            //param.Set()
        }

        public enum ParameterForm
        {
            [StringValue("Instance")]
            Instance = 1,
            [StringValue("Type")]
            Type = 2,
            [StringValue("None")]
            None = 3,
            [StringValue("Invalid")]
            Invalid = 4
        }
    }


}
