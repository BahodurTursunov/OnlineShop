using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace BaseLibrary.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, AuthResponseDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

            CreateMap<Product, ProductDTO>();

            CreateMap<CreateProductDTO, Product>();

            CreateMap<User, UserDTO>();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src=>src.Password));

            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();

        }
    }
}
