using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace GreySMITH.Revit.Wrappers
{
    public abstract partial class AbstractCommand : IExternalCommand
    {
        protected TransactionAttribute _transactionAttribute =      new TransactionAttribute(TransactionMode.Manual);
        protected RegenerationAttribute _regenerationAttribute =    new RegenerationAttribute(RegenerationOption.Manual);

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

        protected AbstractCommand(
            ExternalCommandData excmd,
            string mainmessage,            
            Autodesk.Revit.DB.ElementSet elemset)
        {
            _mainMessage = mainmessage;
            _externalCMD = excmd;
            _elementSet = elemset;
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
            if(
            _mainMessage != mainmessage ||
            _externalCMD != excmd ||
            _elementSet  != elemset)
            {
                _mainMessage = mainmessage;
                _externalCMD = excmd;
                _elementSet = elemset;
            }

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
                Console.WriteLine("Command failed because of an exception.");
            }

            return Result.Failed;
        }

        public abstract Result Work();

    }
}
