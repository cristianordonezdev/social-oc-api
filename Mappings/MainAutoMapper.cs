using AutoMapper;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.DTO.Posts;

namespace social_oc_api.Mappings
{
    public class MainAutoMapper : Profile
    {
        public MainAutoMapper()
        {
            // origin - destination
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.ImageProfile, opt => opt.MapFrom(src => src.ImageProfile.FilePath))
                .ReverseMap();

            CreateMap<PostCreateDto, Post>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();

            // Mapeo del Post a PostDto
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.PostImages));

            // Mapeo del File a FileDto (solo Id y FilePath)
            CreateMap<Image, ImageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));
        }
    }
}
