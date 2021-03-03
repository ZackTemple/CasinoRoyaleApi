using System;
using System.Collections.Generic;
using System.Linq;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;
using Casino_Royale_Api.Services;
using FluentAssertions;
using Xunit;

namespace Casino_Royale_Api.Tests.Services
{
    public class GameManagerTests
    {
        private GameManager _gameManager;
        private CasinoTable TestCasinoTable { get; set; }
        private PlayerBetViewModel TestPlayerBetViewModel { get; set; }
        private PlayerModel TestPlayerModel { get; set; }

        public GameManagerTests()
        {
            _gameManager = new GameManager();
            
            TestPlayerModel = new PlayerModel()
            {
                Username = "MichaelScott",
                CurrentMoney = 100,
                TotalEarned = 0,
                TotalLost = 0,
                Active = true
            };
            TestPlayerBetViewModel = new PlayerBetViewModel {Bet = 5, Player = TestPlayerModel};
            TestCasinoTable = new CasinoTable(TestPlayerBetViewModel);
        }

        [Fact]
        public void StartGame_CreatesNewCasinoTableAndDealsCards()
        {
            // Act
            var casinoTable = _gameManager.StartNewGame(TestPlayerBetViewModel);
            
            // Assert
            casinoTable.Player.Username.Should().Be(TestPlayerBetViewModel.Player.Username);
            casinoTable.Player.CurrentBet.Should().Be(TestPlayerBetViewModel.Bet);
            casinoTable.Player.TotalEarned.Should().Be(TestPlayerBetViewModel.Player.TotalEarned);
            casinoTable.Player.TotalLost.Should().Be(TestPlayerBetViewModel.Player.TotalLost);

            var expectedNewWalletAmount = TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet;

            
            casinoTable.Player.Cards.Should().HaveCount(2);
            casinoTable.Dealer.Cards.Should().HaveCount(1);
        }
        
        [Fact]
        public void StartGame_ThrowsExceptionIfGivenNull()
        {
            // Arrange
            CasinoTable casinoTable;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => casinoTable = _gameManager.StartNewGame(null));
        }

        [Fact]
        public void DealNewCard_ShouldReturnCardNotOnTable()
        {
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "5", Weight = 5},
                new Card(){Suit = "Spades", Value = "J", Weight = 10}
            };
            
            TestCasinoTable.Dealer.Cards = new List<Card>()
            {
                new Card(){Suit = "Diamonds", Value = "8", Weight = 8}
            };

            // We can create 100 cards, and none of them should cards on the table
            var cardList = new List<Card>();
            for (int i = 0; i < 100; i++)
            {
                cardList.Add(_gameManager.DealNewCard(TestCasinoTable));
            }
            
            Assert.Empty(cardList.Intersect(TestCasinoTable.Player.Cards));
            Assert.Empty(cardList.Intersect(TestCasinoTable.Dealer.Cards));
        }
        

        [Fact]
        public void CalculateScore_ShouldReturnScoreOfCards()
        {
            List<Card> cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "5", Weight = 5},
                new Card(){Suit = "Spades", Value = "J", Weight = 10}
            };

            int score = _gameManager.CalculateScore(cards);

            score.Should().Be(15);
        }
        
        [Fact]
        public void CalculateScore_ThrowsExceptionIfGivenNull()
        {
            // Arrange
            int score;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => score = _gameManager.CalculateScore(null));
        }


        [Fact]
        public void HandleAces_ShouldChangeValueOfAceTo1ToAvoidBust()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "5", Weight = 5},
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "A", Weight = 11},
            };

            // Act
            _gameManager.HandleAces(TestCasinoTable.Player);
            var updatedAceCard = TestCasinoTable.Player.Cards.Where(card => card.Value == "A").ToList()[0];

            // Assert
            TestCasinoTable.Player.Score.Should().Be(16);
            updatedAceCard.Weight.Should().Be(1);
        }
        
        [Fact]
        public void HandleAces_ShouldDoNothingIfPlayerScoreLessThan21()
        {
            // Arange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "A", Weight = 11},
            };

            // Act
            _gameManager.HandleAces(TestCasinoTable.Player);
            var aceCard = TestCasinoTable.Player.Cards.Where(card => card.Value == "A").ToList()[0];

            // Assert
            TestCasinoTable.Player.Score.Should().Be(21);
            aceCard.Weight.Should().Be(11);
        }


        [Fact]
        public void EndGameFromUserBust_FinishesGameAndCleansUp()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "3", Weight = 3},
                new Card(){Suit = "Spades", Value = "10", Weight = 10},
            };
            TestCasinoTable.Player.Score = 23;

            // Act
            _gameManager.EndGameFromUserBust(TestCasinoTable);

            // Assert
            TestCasinoTable.Result.Should().Be("Bust");
            TestCasinoTable.Player.TotalLost.Should().Be(
                TestPlayerBetViewModel.Player.TotalLost + TestPlayerBetViewModel.Bet
            );
        }
        
        [Fact]
        public void EndGameFromUserBust_ThrowsExceptionIfGivenNull()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _gameManager.CalculateScore(null));
        }


        [Fact]
        public void FinishGame_AwardsPlayerIfPlayerWins()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "A", Weight = 1},
                new Card(){Suit = "Diamonds", Value = "8", Weight = 8},
            };
            TestCasinoTable.Player.Score = 19;
            
            // Have to set CurrentMoney because we did not subtract bet from CurrentMoney 
            // in constructor
            TestCasinoTable.Player.CurrentMoney = 
                TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet;

            TestCasinoTable.Dealer.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "J", Weight = 10},
                new Card(){Suit = "Hearts", Value = "8", Weight = 8},
            };
            TestCasinoTable.Dealer.Score = 17;
            
            // Act
            _gameManager.FinishGame(TestCasinoTable);

            // Assert
            TestCasinoTable.Result.Should().Be("PlayerWins");
            Assert.Equal(
                2 * TestPlayerBetViewModel.Bet + TestPlayerBetViewModel.Player.TotalEarned,
                TestCasinoTable.Player.TotalEarned
                );
        }
        
        [Fact]
        public void FinishGame_DoesNotAwardPlayerIfDealerWins()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "7", Weight = 7},
            };
            TestCasinoTable.Player.Score = 17;
            
            TestCasinoTable.Dealer.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "J", Weight = 10},
                new Card(){Suit = "Hearts", Value = "A", Weight = 11},
            };
            TestCasinoTable.Dealer.Score = 21;
            
            // Have to set CurrentMoney because we did not subtract bet from CurrentMoney 
            // in constructor
            TestCasinoTable.Player.CurrentMoney = 
                TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet;

            // Act
            _gameManager.FinishGame(TestCasinoTable);

            // Assert
            TestCasinoTable.Result.Should().Be("DealerWins");
            TestCasinoTable.Player.TotalLost.Should().Be(
                TestPlayerBetViewModel.Bet + TestPlayerBetViewModel.Player.TotalLost
            );
            Assert.Equal(
                TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet,
                TestCasinoTable.Player.CurrentMoney
                );
        }
        
        [Fact]
        public void FinishGame_ReturnsPlayersBetIfTie()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "J", Weight = 10},
                new Card(){Suit = "Spades", Value = "A", Weight = 11},
            };
            TestCasinoTable.Player.Score = 21;
            
            // Have to set CurrentMoney because we did not subtract bet from CurrentMoney 
            // in constructor
            TestCasinoTable.Player.CurrentMoney = 
                TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet;
            
            TestCasinoTable.Dealer.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "J", Weight = 10},
                new Card(){Suit = "Diamonds", Value = "A", Weight = 11},
            };
            TestCasinoTable.Dealer.Score = 21;

            // Act
            _gameManager.FinishGame(TestCasinoTable);

            // Assert
            TestCasinoTable.Result.Should().Be("Tie");
            Assert.Equal(
                TestPlayerBetViewModel.Player.CurrentMoney, 
                TestCasinoTable.Player.CurrentMoney
                );
        }
    }
}