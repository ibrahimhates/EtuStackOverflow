using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class PasswordReset : IEntity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AuthField { get; set; }
        public string? RefCode { get; set; }
        public DateTime ExpiresDate { get; set; }
        public User User { get; set; }
    }
}
