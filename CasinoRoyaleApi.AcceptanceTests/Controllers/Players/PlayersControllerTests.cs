using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Casino_Royale_Api.Entities;
using CasinoRoyaleApi.AcceptanceTests.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CasinoRoyaleApi.AcceptanceTests.Controllers.Players
{
    [Collection(TestBase.TestCollectionName)]
    public class PlayersControllerTests : PlayersControllerTestBase
    {
        private readonly Fixture _fixture = new Fixture();
        public PlayersControllerTests(TestFixture testFixture) : base(testFixture)
        {
        }
        

        [Fact]
        public async Task GetPlayers_ReturnsListOfPlayers()
        {
            var playersResponse = await GetPlayersAsync();
            var playersString = await playersResponse.Content.ReadAsStringAsync();
            var deserializePlayers = JsonConvert.DeserializeObject<IEnumerable<Player>>(playersString);

            Assert.Equal(HttpStatusCode.OK, playersResponse.StatusCode);
            deserializePlayers.Should().BeEquivalentTo(Players.Select(x => (PlayerModel) x));
        }
        
        [Fact]
        public async Task GetPlayerByUsername_ReturnsPlayer()
        {
            var expectedPlayer = Players.ToList()[0];
            var response = await GetPlayerByUsernameAsync(expectedPlayer.Username);
            var playerString = await response.Content.ReadAsStringAsync();
            var deserializedPlayer = JsonConvert.DeserializeObject<Player>(playerString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            deserializedPlayer.Should().BeEquivalentTo((PlayerModel) expectedPlayer);
        }
        
        [Fact]
        public async Task GetNonExistentPlayerByUsername_ReturnsNotFound()
        {
            var playersResponse = await GetPlayerByUsernameAsync("bob123456789");

            Assert.Equal(HttpStatusCode.NotFound, playersResponse.StatusCode);
        }

        
        [Fact]
        public async Task RemovePlayer_ReturnsSuccessWithMessage()
        {
            var expectedPlayer = Players.ToList()[0];

            var getResponseBeforeDelete = await GetPlayerByUsernameAsync(expectedPlayer.Username);
            var deleteResponse = await RemovePlayerAsync(expectedPlayer.Username);
            var response = await deleteResponse.Content.ReadAsStringAsync();

            var failedGetResponse = await GetPlayerByUsernameAsync(expectedPlayer.Username);
            
            Assert.Equal(HttpStatusCode.OK, getResponseBeforeDelete.StatusCode);

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            response.Should().Contain(expectedPlayer.Username);
            Assert.Equal(HttpStatusCode.NotFound, failedGetResponse.StatusCode);
        }
        
        [Fact]
        public async Task RemoveNonExistentPlayer_ShouldRespondWithNotFound()
        {
            var username = "kkjashdfytkegffaurysdgfkwjr76545678ijnasydbvhf";

            var response = await RemovePlayerAsync(username);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        
        [Fact]
        public async Task PostPlayer_UpdatesDBWithNewPlayer()
        {
            var newPlayer = _fixture.Create<PlayerModel>();

            // Get player that doesn't exist, should fail
            var getResponseBeforePost = await GetPlayerByUsernameAsync(newPlayer.Username);

            Assert.Equal(HttpStatusCode.NotFound, getResponseBeforePost.StatusCode);
            
            // POST Player
            var postResponse = await AddPlayerAsync(newPlayer.Username);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
            
            //GET Player to make sure player now exists in DB
            var getPlayerResponse = await GetPlayerByUsernameAsync(newPlayer.Username);
            var playerString = await getPlayerResponse.Content.ReadAsStringAsync();
            var playerObject = JsonConvert.DeserializeObject(playerString);
            Assert.Equal(HttpStatusCode.OK, getPlayerResponse.StatusCode);
        }
        
        [Fact]
        public async Task PostExistingPlayer_ReturnsBadRequest()
        {
            var existingPlayer = Players.ToList()[0];
            
            var response = await AddPlayerAsync(existingPlayer.Username);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        
        [Fact]
        public async Task PutPlayer_ShouldReturnOK()
        {
            var updatedPlayer = Players.ToList()[0];
            updatedPlayer.CurrentMoney += 500;
            updatedPlayer.TotalEarned += 500;
            updatedPlayer.TotalLost += 500;
            
            var response = await UpdatePlayerAsync((PlayerModel)updatedPlayer);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Player>(responseString);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            responseObject.Should().BeEquivalentTo((PlayerModel)updatedPlayer);
        }

        [Fact]
        public async Task PutNonExistingPlayer_ShouldReturnBadRequest()
        {
            var player = _fixture.Create<PlayerModel>();

            var response = await UpdatePlayerAsync(player);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}