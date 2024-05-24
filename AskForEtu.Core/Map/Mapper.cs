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

            CreateMap<User, UserListDto>();
            CreateMap<UserProfileUpdateDto,User>();
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count()));

            CreateMap<Question, QuestionListDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => src.User.ProfilePhoto));

            CreateMap<Question, QuestionDetailDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => src.User.ProfilePhoto));

            CreateMap<CreateQuestionDto,Question>();
        }
    }
}
