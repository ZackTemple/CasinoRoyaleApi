using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;
using CasinoRoyaleApi.AcceptanceTests.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CasinoRoyaleApi.AcceptanceTests.Controllers.Blackjack
{
    [Collection(TestBase.TestCollectionName)]
    public class BlackjackControllerTests : BlackjackControllerTestBase
    {
        private PlayerModel TestPlayerModel { get; set; }
        private CasinoTable TestCasinoTable { get; set; }
        private PlayerBetViewModel TestPlayerBetViewModel { get; set; }

        public BlackjackControllerTests(TestFixture testFixture) : base(testFixture)
        {
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
        public async Task StartBlackjackGame_ReturnsNewCasinoTable()
        {
            // Act
            var casinoTableResponseMessage = await StartBlackjackGame(TestPlayerBetViewModel);
            var casinoTable = await DeserializeResponse(casinoTableResponseMessage);

            // Assert
            Assert.Equal(TestPlayerBetViewModel.Player.Username, casinoTable.Player.Username);
            Assert.Equal(TestPlayerBetViewModel.Bet, casinoTable.Player.CurrentBet);
            casinoTable.Player.Cards.Count.Should().Be(2);
            casinoTable.Dealer.Cards.Count.Should().Be(1);

            // Assert
            var newPlayerMoney = TestPlayerBetViewModel.Player.CurrentMoney - TestPlayerBetViewModel.Bet;
            Assert.Equal(newPlayerMoney, casinoTable.Player.CurrentMoney);
            
            //Assert
            Assert.Null(casinoTable.Result);
        }

        [Fact]
        public async Task StartBlackjackGame_Returns400IfBetIsInvalid()
        {
            TestPlayerBetViewModel.Bet = 50000;

            var casinoTableResponseMessage = await StartBlackjackGame(TestPlayerBetViewModel);
            
            Assert.Equal(HttpStatusCode.BadRequest, casinoTableResponseMessage.StatusCode);
        }

        [Fact]
        public async Task StartBlackjackGame_Returns400IfPlayerIsNull()
        {
            TestPlayerBetViewModel.Player = null;
            
            var casinoTableResponseMessage = await StartBlackjackGame(TestPlayerBetViewModel);
            
            Assert.Equal(HttpStatusCode.BadRequest, casinoTableResponseMessage.StatusCode);
        }
        

        [Fact]
        public async Task DealCardToPlayer_ShouldAddACardToPlayersHand()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "5", Weight = 5},
                new Card(){Suit = "Diamonds", Value = "2", Weight = 2}
            };
            
            // Act
            var casinoTableResponseMessage = await DealCardToPlayer(TestCasinoTable);
            var casinoTable = await DeserializeResponse(casinoTableResponseMessage);
            
            // Assert
            casinoTable.Player.Cards.Count.Should().Be(3);
        }
        
        [Fact]
        public async Task DealCardToPlayer_ShouldAddResultIfPlayerBusts()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "10", Weight = 10},
                new Card(){Suit = "Diamonds", Value = "A", Weight = 1},
                new Card(){Suit = "Diamonds", Value = "10", Weight = 10}
            };
            
            // Act
            var casinoTableResponseMessage = await DealCardToPlayer(TestCasinoTable);
            var casinoTable = await DeserializeResponse(casinoTableResponseMessage);
            
            // Assert
            casinoTable.Player.Cards.Count.Should().Be(4);
            Assert.True(casinoTable.Player.Score > 21);
            casinoTable.Result.Should().Be("Bust");
        }
        
        [Fact]
        public async Task DealCardToPlayer_Returns400IfTableIsNull()
        {
            TestCasinoTable = null;
            
            var casinoTableResponseMessage = await DealCardToPlayer(TestCasinoTable);
            
            Assert.Equal(HttpStatusCode.BadRequest, casinoTableResponseMessage.StatusCode);
        }
        
        
        [Fact]
        public async Task FinishGame_LetsDealerPlayAndFinalizesGame()
        {
            // Arrange
            TestCasinoTable.Player.Cards = new List<Card>()
            {
                new Card(){Suit = "Hearts", Value = "10", Weight = 10},
                new Card(){Suit = "Diamonds", Value = "A", Weight = 11}
            };
            TestCasinoTable.Player.Score = 21;
            
            TestCasinoTable.Dealer.Cards = new List<Card>()
            {
                new Card(){Suit = "Spades", Value = "8", Weight = 8}
            };
            TestCasinoTable.Dealer.Score = 8;

            
            // Act
            var casinoTableResponseMessage = await FinishGame(TestCasinoTable);
            var casinoTable = await DeserializeResponse(casinoTableResponseMessage);
            
            // Assert
            casinoTable.Player.Cards.Count.Should().Be(2);
            Assert.True(casinoTable.Dealer.Cards.Count > 1);
            Assert.NotNull(casinoTable.Result);
            
            // Must be true since Player has blackjack (result can be Tie or Player wins, both cases add to CurrentMoney)
            Assert.True(casinoTable.Player.CurrentMoney > TestCasinoTable.Player.CurrentMoney);
            
            if (casinoTable.Result == "Tie")
            {   // If tie, player didn't win anything, so TotalEarned doe snot go up
                Assert.Equal(TestCasinoTable.Player.TotalEarned, casinoTable.Player.TotalEarned);
            }
            else
            {   // If not a tie here, Player won (has blackjack), so TotalEarned increases
                Assert.True(casinoTable.Player.TotalEarned > TestCasinoTable.Player.TotalEarned);
            }
        }
        
        [Fact]
        public async Task FinishGame_Returns400IfTableIsNull()
        {
            TestCasinoTable = null;
            
            var casinoTableResponseMessage = await FinishGame(TestCasinoTable);
            
            Assert.Equal(HttpStatusCode.BadRequest, casinoTableResponseMessage.StatusCode);
        }
    }
}