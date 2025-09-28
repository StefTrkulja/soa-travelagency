// StakeholdersService/Mapping/AccountProfile.cs
using AutoMapper;
using StakeholdersService.Domain;
using StakeholdersService.DTO;

namespace StakeholdersService.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // User -> AccountDto
            CreateMap<User, AccountDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Blocked, opt => opt.MapFrom(s => s.Blocked));

            // User -> UserProfileDto
            CreateMap<User, UserProfileDto>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

            // UpdateUserProfileDto -> User (for patching)
            CreateMap<UpdateUserProfileDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // (opciono) Ako ćeš nekad mapirati nazad:
    
        }
    }
}
