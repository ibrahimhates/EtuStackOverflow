

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Faculty : IEntity<byte>
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public ICollection<Major> Majors { get; set; }
        public ICollection<User> Users{ get; set; }
    }
}
