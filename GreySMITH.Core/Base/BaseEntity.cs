using GreySMITH.Core.Intefaces;

namespace GreySMITH.Core.Base
{
    public abstract class BaseEntity : IEntity
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
    }
}