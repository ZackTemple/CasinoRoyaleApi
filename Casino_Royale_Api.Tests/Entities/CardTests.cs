using Casino_Royale_Api.Models;
using FluentAssertions;
using Xunit;

namespace Casino_Royale_Api.Tests
{
    public class CardTests
    {
        [Fact]
        public void Card_CallingConstructorShouldCallHelperfunctions()
        {
            var myCard = new Card();

            Assert.IsType<string>(myCard.Suit);
            Assert.IsType<string>(myCard.Value);
            Assert.IsType<int>(myCard.Weight);
        }
    }
}