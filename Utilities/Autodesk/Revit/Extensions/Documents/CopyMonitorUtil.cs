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

using GreySMITH.Utilities.Autodesk.Revit.Extensions.Documents;

namespace GreySMITH.Utilities.Autodesk.Revit.Extensions.Documents
{
    /// <summary>
    /// Class of methods which deal with extending CopyMonitor functionality
    /// </summary>
    public static class CopyMonitorUtil
    {
        #region Copy Monitor Utility
        /// <summary>
        /// Copies in elements of a specified type into the document
        /// </summary>
        /// <param name="origdoc">Current Document</param>
        /// <param name="lnkfullpath">Path of the document which elements should be copied from</param>
        /// <param name="CM_elemtype">Type of the elements that should be copied</param>
        public static FilteredElementCollector CopyLinkedObjects(this Document origdoc,
            string lnkfullpath,
            BuiltInCategory CM_elemtype)
        {
            // Select the Link which should be Copy Monitored
            IEnumerable<Document> docset = origdoc.Application.Documents.Cast<Document>();
            Document linkeddoc = (from d in docset
                                  where d.PathName.Equals(lnkfullpath)
                                  select d).Single();

            // Filter through all the elements in the linked document of the selected type
            FilteredElementCollector elemlist =
                new FilteredElementCollector(linkeddoc).OfCategory(CM_elemtype);

            // Return group/collection of elements
            return elemlist;
        }

        public static Result PasteLinkedObjects(
            this Document curdoc,
            FilteredElementCollector fecoflinkedobjs,
            Document linkeddoc,
            ExternalCommandData excmd)
        {
            FamilySymbol famsym = null;
            FamilyInstance faminst = null;
            Element temphost = null;

            // acquire the category of the elements
            BuiltInCategory categoryoffec = (BuiltInCategory)fecoflinkedobjs.FirstElement().Category.Id.IntegerValue;


            // load in any new families from the linked file
            curdoc.LoadFamilyDirect(fecoflinkedobjs, excmd);

            switch(categoryoffec)
            {
                case BuiltInCategory.OST_ElectricalFixtures:
                    
                    break;
            }

            // set up for incoming hosts
            ElementReferenceType hosttype = ElementReferenceType.REFERENCE_TYPE_NONE;
            switch(hosttype)
            {
                default:
                    //Element has no host
                    break;
                case ElementReferenceType.REFERENCE_TYPE_SURFACE:
                    break;
                case ElementReferenceType.REFERENCE_TYPE_INSTANCE:
                    break;
            }
            

            return Result.Succeeded;
        }
        #endregion
    }
}
