using Xunit;
using Casino_Royale_Api.Entities;

namespace Casino_Royale_Api.Tests
{
    public class PlayerEntityTests
    {
        [Fact]
        public void CreateDefaultPlayer()
        {
            var newPlayer = Player.DefaultPlayerFactory("bob");
            var expectedPlayer = new Player(
                "bob",
                100,
                0,
                0,
                false
            );

            Assert.Equal(newPlayer.Username, expectedPlayer.Username);
            Assert.Equal(newPlayer.CurrentMoney, expectedPlayer.CurrentMoney);
            Assert.Equal(newPlayer.TotalEarned, expectedPlayer.TotalEarned);
            Assert.Equal(newPlayer.TotalLost, expectedPlayer.TotalLost);
            Assert.Equal(newPlayer.Active, expectedPlayer.Active);
        }
    }
}
