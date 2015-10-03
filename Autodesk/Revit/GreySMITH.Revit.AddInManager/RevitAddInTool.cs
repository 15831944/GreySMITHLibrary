using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.RevitAddIns;

namespace GreySMITH.Revit.AddInManager
{
    public static class RevitAddInTool
    {
        private const string VendorId =                  "GreySMITH";
        private const string ManifestName =              "{0}.addin";
        private const string AddinFullName =             "GreySMITH.Revit.Commands.{0}.dll";
        private const string AddinClassName =            "GreySMITH.Revit.{0}";

        public static IEnumerable<string> GetAllAddInLocations()
        {
            return RevitProductUtility.GetAllInstalledRevitProducts().Select(p => p.CurrentUserAddInFolder);
        }

        public static string GetInstallationFolder(string revitProductName)
        {
            return (from revitProgram in RevitProductUtility.GetAllInstalledRevitProducts()
                    where revitProgram.Name.Equals(revitProductName)
                    select revitProgram.InstallLocation).First();
        }

        public static void CreateAddinManifest(RevitVersion revitVersion, string directory)
        {
            RevitAddInManifest revitManifest = new RevitAddInManifest();
            FileInfo fileInfo = new FileInfo(Path.Combine());

        }

        public static void FindAllCommands()
        {
            
        }

    }
}
