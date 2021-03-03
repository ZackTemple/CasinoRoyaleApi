using AutoMapper;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerModel>()
                .ReverseMap();
                    // .ForMember(c => c.id, opt => opt.Ignore());
        }
    }
}
