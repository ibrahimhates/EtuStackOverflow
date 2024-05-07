using AskForEtu.Core.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskForEtu.Core.Entity
{
    public class DisLike : IEntity<long>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public long CommentId { get; set; }
        public User User { get; set; }
        public Comment Comment { get; set; }
    }
}
