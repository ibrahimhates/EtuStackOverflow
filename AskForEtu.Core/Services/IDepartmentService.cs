using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
namespace AskForEtu.Core.Services
{
    public interface IDepartmentService
    {
        Task<Response<List<FacultyDto>>> GetAllFacultyAsync();
        Task<Response<List<MajorDto>>> GetAllMajowByIdAsync(byte facultyId);
    }
}
