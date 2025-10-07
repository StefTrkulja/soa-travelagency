using AutoMapper;
using StakeholdersService.Domain;
using StakeholdersService.DTO;

namespace StakeholdersService.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<User, AccountDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Blocked, opt => opt.MapFrom(s => s.Blocked));

            // User -> UserProfileDto
            CreateMap<User, UserProfileDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Surname, opt => opt.MapFrom(s => s.Surname))
                .ForMember(d => d.ProfilePicture, opt => opt.MapFrom(s => s.ProfilePicture))
                .ForMember(d => d.Biography, opt => opt.MapFrom(s => s.Biography))
                .ForMember(d => d.Motto, opt => opt.MapFrom(s => s.Motto));

        }
    }
}
