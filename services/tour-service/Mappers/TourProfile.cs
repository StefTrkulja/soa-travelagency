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
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TourTags.Select(tt => tt.Tag.Name).ToList()))
            .ForMember(dest => dest.KeyPoints, opt => opt.MapFrom(src => src.KeyPoints.OrderBy(kp => kp.Order)))
            .ForMember(dest => dest.TransportTimes, opt => opt.MapFrom(src => src.TransportTimes));

        CreateMap<CreateTourRequestDto, Tour>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => (TourDifficulty)src.Difficulty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TourStatus.Draft))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.TourTags, opt => opt.Ignore())
            .ForMember(dest => dest.KeyPoints, opt => opt.Ignore())
            .ForMember(dest => dest.TransportTimes, opt => opt.Ignore())
            .ForMember(dest => dest.DistanceInKm, opt => opt.MapFrom(src => src.DistanceInKm));

        CreateMap<TourKeyPoint, TourKeyPointDto>();
        CreateMap<CreateTourKeyPointRequestDto, TourKeyPoint>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TourId, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Tour, opt => opt.Ignore());

        CreateMap<TourTransportTime, TourTransportTimeDto>()
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType.ToString()));
        CreateMap<CreateTourTransportTimeRequestDto, TourTransportTime>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TourId, opt => opt.Ignore())
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => (TransportType)src.TransportType))
            .ForMember(dest => dest.Tour, opt => opt.Ignore());
    }
}