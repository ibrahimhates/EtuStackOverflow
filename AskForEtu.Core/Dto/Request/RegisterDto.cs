
namespace AskForEtu.Core.Dto.Request
{
    public record RegisterDto(
            string Name,
            string SurName,
            string Email,
            byte FacultyId,
            byte MajorId,
            byte Grade,
            string UserName,
            string Password,
            string PasswordAgain
        );
}
