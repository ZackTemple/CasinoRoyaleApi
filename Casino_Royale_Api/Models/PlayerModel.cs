using System.Collections.Generic;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class PlayerModel
    {
        #nullable enable
        public string Username { get; set; }
        public double? CurrentMoney { get; set; }
        public double? TotalEarned { get; set; }
        public double? TotalLost { get; set; }
        public bool? Active { get; set; }

        public static explicit operator PlayerModel(Player entity)
        {
            return new PlayerModel()
            {
                Username = entity.Username,
                CurrentMoney = entity.CurrentMoney,
                TotalEarned = entity.TotalEarned,
                TotalLost = entity.TotalLost,
                Active = entity.Active,
            };
        }
    }
}
