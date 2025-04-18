using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace BaseLibrary.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, AuthResponseDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
        }
    }
}
