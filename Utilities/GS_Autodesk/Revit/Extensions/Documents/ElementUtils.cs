using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

using GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Applications;

namespace GreySMITH.Utilities.GS_Autodesk.Revit.Extensions.Documents
{
    public static class ElementUtils
    {
        public static Element FindHost(this Document curdoc, FamilyInstance faminstance )
        {
            Element host = null;

            #region Find the Revit Link*
            // get the revitlinkinstance from the current document
            FilteredElementCollector newlinkfec = new FilteredElementCollector(curdoc).OfClass(typeof(RevitLinkInstance));
            RevitLinkInstance rvtlink_other = (from posslink in newlinkfec
                                               where (posslink as RevitLinkInstance).GetLinkDocument().PathName.ToString().Equals(faminstance.Host.Document.PathName.ToString())
                                               select (posslink as RevitLinkInstance)).Single();
            #endregion

            #region Finding the Host in the Linked Document
            // for face based elements
            // make a list of elements in the linked document which match the host's type
            // in this test case, these should return walls
            FilteredElementCollector linkdocfec = new FilteredElementCollector(faminstance.Host.Document);
            linkdocfec.OfClass(faminstance.Host.GetType());

            // find the host in the list by comparing the UNIQUEIDS
            host = (from posshost in linkdocfec
                    where posshost.UniqueId.ToString().Equals(faminstance.Host.UniqueId.ToString())
                    select posshost).First();
            #endregion

            return host;
        }
    }
}
