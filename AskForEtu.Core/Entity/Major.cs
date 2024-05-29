

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Major : IEntity<byte>
    {
        public byte Id { get; set; }

        public byte FacultyId { get; set; }

        public string Name { get; set; }

        public Faculty Faculty { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
