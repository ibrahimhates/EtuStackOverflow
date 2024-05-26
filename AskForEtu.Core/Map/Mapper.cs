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
                .ForPath(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count()));

            CreateMap<Question, QuestionListDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => src.User.ProfilePhoto));

            CreateMap<Question, QuestionDetailDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => src.User.ProfilePhoto));

            CreateMap<Question, QuestionForProfileDto>();

            CreateMap<Comment, CommentDto>()
                .ForPath(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForPath(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => src.User.ProfilePhoto))
                .ForPath(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count()))
                .ForPath(dest => dest.DisLikeCount, opt => opt.MapFrom(src => src.DisLikes.Count()));

            CreateMap<CreateCommentDto, Comment>();

            CreateMap<CreateQuestionDto,Question>();
        }
    }
}
