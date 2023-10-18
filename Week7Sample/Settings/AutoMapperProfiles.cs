using AutoMapper;
using Week7Sample.Model;

namespace Week7Sample.Settings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddUserDto, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email))
                .ReverseMap();
            CreateMap<User, UserReturnDto>().ReverseMap();
        }
    }
}
