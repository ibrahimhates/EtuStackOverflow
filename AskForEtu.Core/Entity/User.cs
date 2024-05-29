using AskForEtu.Core.Entity.Base;

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
        public string? VerifyEmailToken { get; set; }
        public Faculty Faculty { get; set; }
        public Major Major { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Report> Reports{ get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<DisLike> DisLikes { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public Token Token { get; set; }
        public PasswordReset PasswordReset { get; set; }
    }
}
