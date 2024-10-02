using AutoMapper;
using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.DTO.Posts;

namespace social_oc_api.Mappings
{
    public class MainAutoMapper : Profile
    {
        public MainAutoMapper()
        {
            // origin - destination
            CreateMap<ApplicationUser, UserDto>().ReverseMap();

            CreateMap<PostCreateDto, Post>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();

            CreateMap<IFormFile, Image>()
                .ForMember(dest => dest.FilePath, opt => opt.Ignore()) // Ignora FilePath por ahora
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src)); // Mapea el IFormFile directamente
        }
    }
}
