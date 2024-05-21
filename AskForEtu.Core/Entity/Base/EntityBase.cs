
namespace AskForEtu.Core.Entity.Base
{
    public abstract class EntityBase
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
