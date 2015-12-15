using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public class AutoCADCommand
    {
        public AutoCADCommand(Document currentDocument)
        {
            Document = currentDocument;
        }
        private static Document Document { get; set; }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static void Execute(params string[] commandstoexecute)
        {
            foreach (string command in commandstoexecute)
            {
                try
                {
                    Logger.Debug("Attempting to {0} the model", command);
                    Document.SendStringToExecute(command + "\n", true, false, true);
                }

                catch(Exception exception)
                {
                    Logger.Error("The program failed while attempting to execute {0}.", command);
                    Logger.Error("The program threw an exception: {0}", exception.Message);
                }
            }
        }
        public static void Purge()
        {
            Execute("-PURGE", "ALL", "*", "N");
        }
        public static void Audit()
        {
            Execute("_Audit", "Y");
        }
        public static void Recover()
        {
            Execute("RECOVER", Document.Name);
        }
    }
}
