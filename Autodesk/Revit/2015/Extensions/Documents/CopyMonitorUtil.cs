using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GreySMITH.Revit.Extensions.Documents
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
            #region Prep
            // acquire the category of the elements and make local variables
            FamilySymbol famsym = null;
            FamilyInstance faminst = null;
            Element temphost = null;
            BuiltInCategory categoryoffec = (BuiltInCategory)fecoflinkedobjs.FirstElement().Category.Id.IntegerValue;

            // load in all new families from the linked file
            curdoc.LoadFamilyDirect(fecoflinkedobjs, excmd);
            #endregion

            // for each instance in the collector

            // look at the host

            // if the host is not in the current document, it is a linked item

            // if item is linked

            // find host in the linked document

            // create a reference plane based on this host in the current document

            // place the item on the reference plane

            // force the item to monitor the item in the linked document?

            

            #region Category Based Operations
            switch (categoryoffec)
            {
                case BuiltInCategory.OST_ElectricalFixtures:
                    
                    break;
            }
            #endregion

            #region Find the Host of object
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
            #endregion


            return Result.Succeeded;
        }
        #endregion
    }
}
