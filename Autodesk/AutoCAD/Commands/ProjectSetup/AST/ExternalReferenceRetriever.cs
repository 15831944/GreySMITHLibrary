using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GreySMITH.Autodesk.AutoCAD.Wrappers;
using NLog;

namespace GreySMITH.Autodesk.AutoCAD
{
    public class ExternalReferenceRetriever : IRetriever<Document ,IEnumerable<BlockTableRecord>>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public IEnumerable<BlockTableRecord> Retrieve(Document document)
        {
            IEnumerable<BlockTableRecord> externalReferences;

            using (Transaction transaction = document.TransactionManager.StartTransaction())
            {
                //Create a list of all xrefs in the file
                //Check the drawing to see if there are any xrefs
                externalReferences =
                    (from BlockTableRecord xrefBlock in
                        (new BlockTableRecordRetriever().Retrieve(document))
                     where xrefBlock.IsFromExternalReference
                     select xrefBlock);

                //Complete the command
                transaction.Commit();
            }

            return externalReferences;
        }
    }
}
