using AskForEtu.Core.Entity.Base;

namespace AskForEtu.Core.Entity
{
    public class Token : IEntity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
    }
}
