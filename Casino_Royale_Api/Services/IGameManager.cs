using System.Collections.Generic;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Services
{
    public interface IGameManager
    {
        public CasinoTable StartNewGame(PlayerBetViewModel model);
        public Card DealNewCard(CasinoTable table);
        public int CalculateScore(List<Card> cards);
        public List<Card> HandleAces(CardHolder cardHolder);
        public CasinoTable EndGameFromUserBust(CasinoTable table);
        public CasinoTable FinishGame(CasinoTable table);
    }
}