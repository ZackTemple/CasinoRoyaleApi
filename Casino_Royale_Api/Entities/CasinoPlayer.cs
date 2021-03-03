using System.Collections.Generic;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class CasinoPlayer : CardHolder
    {
        #nullable enable
        public string Username { get; set; }
        public double? CurrentMoney { get; set; }
        public double? TotalEarned { get; set; }
        public double? TotalLost { get; set; }
        public bool? Active { get; set; }
        public double CurrentBet { get; set; }

    public static explicit operator CasinoPlayer(PlayerModel model)
        {
            return new CasinoPlayer()
            {
                Username = model.Username,
                CurrentMoney = model.CurrentMoney,
                TotalEarned = model.TotalEarned,
                TotalLost = model.TotalLost,
                Active = model.Active,
                CurrentBet = 0,
                Cards = new List<Card>(),
                Score = 0
            };
        }
    }
}