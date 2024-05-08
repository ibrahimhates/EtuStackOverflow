using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.Entity;
using AutoMapper;

namespace AskForEtu.Core.Map
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<RegisterDto, User>();

            CreateMap<Faculty, FacultyDto>();
            CreateMap<Major, MajorDto>();
        }
    }
}
