

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Report : IEntity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long CommentId { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public Comment Comment { get; set; }
    }
}
