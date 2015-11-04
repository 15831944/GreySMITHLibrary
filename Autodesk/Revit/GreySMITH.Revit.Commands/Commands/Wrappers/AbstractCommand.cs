using System;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using NLog;
using GreySMITH.Revit.Commands;

namespace GreySMITH.Revit.Commands.Wrappers
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public abstract partial class AbstractCommand : IExternalCommand
    {
        private static Logger logger;
        public string PanelName;
        public string CommandName;
        public string AssemblyLocation;
        public string Description;
        public string ClassName;

        protected ExternalCommandData _externalCMD
        {
            get;
            set;
        }
        protected string _mainMessage
        {
            get;
            set;
        }
        protected Autodesk.Revit.DB.ElementSet _elementSet
        {
            get;
            set;
        }
        protected UIApplication UiApplication
        {
            get
            {
                return _externalCMD.Application;
            }
        }
        protected Document CurrentDocument
        {
            get { return UiApplication.ActiveUIDocument.Document; }
        }
        protected UIDocument UiDocument
        {
            get { return UiApplication.ActiveUIDocument; }
        }
        protected AbstractCommand(
            ExternalCommandData excmd,
            string mainmessage,            
            Autodesk.Revit.DB.ElementSet elemset,
            string commandName,
            string panelName,
            string className,
            string assemblyLocation,
            string description)
        {
            _mainMessage = mainmessage;
            _externalCMD = excmd;
            _elementSet = elemset;
            CommandName = commandName;
            PanelName = panelName;
            ClassName = this.GetType().Name;
            Description = description;
            AssemblyLocation = Assembly.GetExecutingAssembly().Location;

            CommandList.Add(this);
        }

        protected AbstractCommand()
        {
            _mainMessage = null;
            _externalCMD = null;
            _elementSet  = null;
        }

        // necessary method that Revit needs to call
        Result IExternalCommand.Execute(
            ExternalCommandData excmd,
            ref string mainmessage,
            Autodesk.Revit.DB.ElementSet elemset)
        {
            // if any of the private fields are incorrect - let Revit re-correct them
            // tried doing this before initializing the fields
//            if(
//            (!_mainMessage.Equals(mainmessage)) ||
//            (!_externalCMD.Equals(excmd)) ||
//            (!_elementSet.Equals(elemset)))
//            {
                _mainMessage = mainmessage;
                _externalCMD = excmd;
                _elementSet = elemset;
//            }

            return Execute();
        }

        // Internal method that allows the Revit method to use the internal method below
        protected Result Execute()
        {
            return Execute(_externalCMD, _mainMessage, _elementSet);
        }

        // Internal method that allows this class to use this private fields it contains
        // without having to set them necessarily.
        protected Result Execute(
            ExternalCommandData excmd,
            string mainmessage,
            Autodesk.Revit.DB.ElementSet elemset)
        {
            try
            {
                // defined in derived classes
                return Work();
            }

            catch
            {
                logger.Debug("Command failed because of an exception");
                TaskDialog.Show("Command Failed",
                    "There was an error behind the scenes that caused the command to fail horribly and die.");
            }

            return Result.Failed;
        }

        public abstract Result Work();

    }
}
