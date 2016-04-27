namespace GreySMITH.Domain.AutoCAD.Wrappers
{
    public interface IRetriever<in TargetSource, out TargetOut>
    {
        TargetOut Retrieve(TargetSource source);
    }
}
