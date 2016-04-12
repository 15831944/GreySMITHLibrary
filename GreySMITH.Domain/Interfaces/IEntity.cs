namespace GreySMITH.Domain.Interfaces
{
    public interface IEntity
    {
        string Name { get; } 
    }

    public class BaseEntity : IEntity
    {
        public string Name { get; private set; }
    }
}