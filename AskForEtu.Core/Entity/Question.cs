﻿using AskForEtu.Core.Entity.Base;
using System.ComponentModel.DataAnnotations;

namespace AskForEtu.Core.Entity
{
    public class Question : EntityBase, IEntity<long>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsSolved { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments{ get; set; }
    }
}
