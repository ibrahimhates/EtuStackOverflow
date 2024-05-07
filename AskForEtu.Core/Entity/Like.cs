

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Like : IEntity<long>
    {
        public long Id { get ; set; }
        public int UserId { get; set; }
        public long CommentId { get; set; }
        public User User { get; set; }
        public Comment Comment { get; set; }
    }
}
