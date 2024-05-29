using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity;
public class UserRole : EntityBase, IEntity<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
