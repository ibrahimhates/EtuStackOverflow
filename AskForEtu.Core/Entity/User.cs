﻿
using AskForEtu.Core.Entity.Base;
using System.ComponentModel.DataAnnotations;

namespace AskForEtu.Core.Entity
{
    public class User : EntityBase, IEntity<int>
    {
        public int Id { get; set; }
        public byte FacultyId { get; set; }
        public byte MajorId { get; set; }
        public byte Grade { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public bool IsActive { get; set; }
        public string FullName => $"{Name} {SurName}";
        public string UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PasswordHash { get; set; }
        public byte[]? ProfilePhoto { get; set; }
        public string Email { get; set; }
        public bool VerifyEmail { get; set; }
        public string VerifyEmailToken { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
