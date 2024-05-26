

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Comment :EntityBase, IEntity<long>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public long QuestionId { get; set; }
        public string Content { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<DisLike> DisLikes { get; set; }
        public ICollection<Report> Reports { get; set; }
        public User User { get; set; }
        public Question Question { get; set; }

    }
}
