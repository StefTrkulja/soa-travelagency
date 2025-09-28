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
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

            // (opciono) Ako ćeš nekad mapirati nazad:
    
        }
    }
}
