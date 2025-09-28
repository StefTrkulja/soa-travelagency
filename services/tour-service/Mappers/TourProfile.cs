using AutoMapper;
using TourService.Domain;
using TourService.DTO;

namespace TourService.Mappers;

public class TourProfile : Profile
{
    public TourProfile()
    {
        CreateMap<Tour, TourDto>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TourTags.Select(tt => tt.Tag.Name).ToList()));

        CreateMap<CreateTourRequestDto, Tour>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => (TourDifficulty)src.Difficulty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TourStatus.Draft))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.TourTags, opt => opt.Ignore());
    }
}