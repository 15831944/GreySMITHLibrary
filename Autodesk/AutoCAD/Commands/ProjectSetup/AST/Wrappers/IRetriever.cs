using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GreySMITH.Autodesk.AutoCAD.Wrappers
{
    public interface IRetriever<in TargetSource, out TargetOut>
    {
        TargetOut Retrieve(TargetSource source);
    }
}
