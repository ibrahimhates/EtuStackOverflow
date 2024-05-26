using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AskForEtu.Core.Dto.Request
{
    public record RegisterDto
    {
        public string Name { get; init; }
        public string SurName { get; init; }
        [EmailAddress]
        public string Email { get; init; }
        public byte FacultyId { get; init; }
        public byte MajorId { get; init; }
        public byte Grade { get; init; }
        [MinLength(5,ErrorMessage = "Kullanici adi uzunlugu en az 5 olmali")]
        public string UserName { get; init; }
        [PasswordPropertyText]
        public string Password { get; init; }
        [Compare("Password",ErrorMessage = "Sifreler uyusmuyor")]
        public string PasswordAgain { get; init; }
    }
}
