using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Entity;
using AutoMapper;

namespace AskForEtu.Core.Map
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<RegisterDto, User>();
        }
    }
}
