using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity;
public class Role : EntityBase, IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserRole> Users { get; set; }
}
