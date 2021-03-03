using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Services
{
    public class GameManager : IGameManager
    {

        public CasinoTable StartNewGame(PlayerBetViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            var table = CreateCasinoTable(model);
            table.Player = SubtractBetFromPlayerWallet(table.Player);
            table = DealCardsToBeginGame(table);

            return table;
        }
        
        private CasinoTable CreateCasinoTable(PlayerBetViewModel model)
        {
            return new CasinoTable(model);
        }
        
        private CasinoPlayer SubtractBetFromPlayerWallet(CasinoPlayer player)
        {
            player.CurrentMoney -= player.CurrentBet;
            return player;
        }

        private CasinoTable DealCardsToBeginGame(CasinoTable table)
        {
            for (int i = 0; i < 2; i++)
            {
                table.Player.Cards.Add(DealNewCard(table));
            }

            table.Player.Score = CalculateScore(table.Player.Cards);
            
            table.Dealer.Cards.Add(DealNewCard(table));
            table.Dealer.Score = CalculateScore(table.Dealer.Cards);

            return table;
        }

        public Card DealNewCard(CasinoTable table)
        {
            var cardsOnTable = GetCardsInPlay(table);
            
            Card newCard = new Card();
            while (cardsOnTable.Contains(newCard))
            {
                newCard = new Card();
            }
            
            return newCard;
        }

        private List<Card> GetCardsInPlay(CasinoTable table)
        {
            // Use attribute for players from table here as well
            var cardsInPlay = new List<Card>();
            
            cardsInPlay.AddRange(table.Player.Cards);
            cardsInPlay.AddRange(table.Dealer.Cards);

            return cardsInPlay;
        }

        public int CalculateScore(List<Card> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }
            
            int totalScore = 0;
            foreach (var card in cards)
            {
                totalScore += card.Weight;
            }

            return totalScore;
        }

        public List<Card> HandleAces(CardHolder cardHolder)
        {
            cardHolder.Score = CalculateScore(cardHolder.Cards);
            while (cardHolder.Score > 21 && cardHolder.Cards.Exists(card => card.Weight == 11))
            {
                var index = cardHolder.Cards.FindIndex(card => card.Weight == 11);
                Card aceCard = cardHolder.Cards[index];
                aceCard.Weight = 1;
                
                cardHolder.Cards.RemoveRange(index, 1);
                cardHolder.Cards.Insert(index, aceCard);

                cardHolder.Score = CalculateScore(cardHolder.Cards);
            }

            return cardHolder.Cards;
        }

        public CasinoTable EndGameFromUserBust(CasinoTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            
            table.Result = Result.Bust.ToString();
            table.Dealer.Score = CalculateScore(table.Dealer.Cards);
            table = ActOnGameResults(table);
            return table;
        }

        private CasinoTable ActOnGameResults(CasinoTable table)
        {
            table.Player = table.Result == Result.PlayerWins.ToString() ? AwardPlayerForWin(table.Player) :
                table.Result == Result.Tie.ToString() ? ReturnPlayerBet(table.Player) :
                SubtractPlayerBetFromTotalLost(table.Player);

            return table;
        }

        private CasinoPlayer AwardPlayerForWin(CasinoPlayer player)
        {
            var multiplier = GetBetMultiplier(player);

            player.CurrentMoney += multiplier * player.CurrentBet;
            player.TotalEarned += multiplier * player.CurrentBet;

            return player;
        }

        private double GetBetMultiplier(CasinoPlayer player)
        {
            if (player.Score == 21 && player.Cards.Count == 2)
            {
                return 2.5;
            }

            return 2;
        }

        private CasinoPlayer ReturnPlayerBet(CasinoPlayer player)
        {
            player.CurrentMoney += player.CurrentBet;

            return player;
        }

        private CasinoPlayer SubtractPlayerBetFromTotalLost(CasinoPlayer player)
        {
            player.TotalLost += player.CurrentBet;
            return player;
        }

        public CasinoTable FinishGame(CasinoTable table)
        {
            table.Dealer = PlayDealersTurn(table);
            table = GetGameResults(table);
            table = ActOnGameResults(table);

            return table;
        }

        private Dealer PlayDealersTurn(CasinoTable table)
        {
            table.Dealer.Cards = HandleAces(table.Dealer);
            table.Dealer.Score = CalculateScore(table.Dealer.Cards);
            
            while (table.Dealer.Score < 17 && table.Dealer.Score <= table.Player.Score) {
                table.Dealer.Cards.Add(DealNewCard(table));
                table.Dealer.Cards = HandleAces(table.Dealer);
                table.Dealer.Score = CalculateScore(table.Dealer.Cards);
            }

            return table.Dealer;
        }

        private CasinoTable GetGameResults(CasinoTable table)
        {
            if (table.Player.Score > table.Dealer.Score || table.Dealer.Score > 21)
            {
                table.Result = Result.PlayerWins.ToString();
            }
            else if (table.Dealer.Score > table.Player.Score)
            {
                table.Result = Result.DealerWins.ToString();
            }
            else if (table.Player.Score == table.Dealer.Score)
            {
                table.Result = Result.Tie.ToString();
            }

            return table;
        }
    }
}