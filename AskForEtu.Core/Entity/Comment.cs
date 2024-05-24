

using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Comment : IEntity<long>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public long QuestionId { get; set; }
        public string Content { get; set; }
        ICollection<Like> Likes { get; set; }
        ICollection<DisLike> DisLikes { get; set; }
        ICollection<User> Reports { get; set; }
        public User User { get; set; }
        public Question Question { get; set; }

    }
}
