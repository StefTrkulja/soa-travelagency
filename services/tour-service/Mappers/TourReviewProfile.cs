using AutoMapper;
using TourService.Domain;
using TourService.DTO;

namespace TourService.Mappers;

public class TourReviewProfile : Profile
{
    public TourReviewProfile()
    {
        CreateMap<TourReview, TourReviewDto>()
            .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Tour != null ? src.Tour.Name : null));

        CreateMap<CreateTourReviewRequestDto, TourReview>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tour, opt => opt.Ignore());

        CreateMap<UpdateTourReviewRequestDto, TourReview>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TourId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.VisitationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Tour, opt => opt.Ignore());
    }
}