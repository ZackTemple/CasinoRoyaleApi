using System.Collections.Generic;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class CasinoTable
    {
        public CasinoPlayer Player { get; set; }
        public Dealer Dealer { get; set; }
        #nullable enable
        public string? Result { get; set; }

        public CasinoTable(PlayerBetViewModel model)
        {
            InitializePlayerInfo(model);

            Dealer = new Dealer {Score = 0};

            Result = null;
        }

        private void InitializePlayerInfo(PlayerBetViewModel model)
        {
            Player = (CasinoPlayer)model.Player;
            Player.CurrentBet = model.Bet;
            Player.Score = 0;
            Player.Cards = new List<Card>();
        }
        
        public CasinoTable()
        {
        }
    }
}