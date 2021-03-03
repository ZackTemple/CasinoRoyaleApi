using Casino_Royale_Api.Entities;
using FluentAssertions;
using Xunit;

namespace Casino_Royale_Api.Tests
{
    public class CasinoTableTests
    {
        [Fact]
        public void CasinoTable_ShouldCreateTableIfGivenPlayerBetViewModel()
        {
            var playerBetViewModel = new PlayerBetViewModel()
            {
                Bet = 5,
                Player = new PlayerModel()
                {
                    Username = "MichaelScott",
                    CurrentMoney = 100,
                    TotalEarned = 0,
                    TotalLost = 0,
                    Active = true
                }
            };
            
            var myTable = new CasinoTable(playerBetViewModel);
            
            Assert.Equal(myTable.Player.Username, playerBetViewModel.Player.Username);
            myTable.Dealer.Should().NotBeNull();
            myTable.Player.CurrentBet.Should().Be(5);
            myTable.Player.Score.Should().Be(0);
        }
    }
}