using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Casino_Royale_Api.Constants;
using Casino_Royale_Api.Entities;
using CasinoRoyaleApi.AcceptanceTests.Helpers;
using Newtonsoft.Json;

namespace CasinoRoyaleApi.AcceptanceTests.Controllers.Players
{
    public class PlayersControllerTestBase : TestBase
    {
        private readonly Fixture _fixture = new Fixture();
        protected IEnumerable<Player> Players;
        
        protected PlayersControllerTestBase(TestFixture testFixture) : base(testFixture)
        {
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            DbContext.ClearPlayersDatabase();
            Players = _fixture
                .Build<Player>()
                .Without(x => x.Id)
                .CreateMany(10);
            
            DbContext.AddRange(Players);
            DbContext.SaveChanges();
        }
        
        protected Task<HttpResponseMessage> GetPlayersAsync()
        {
            var requestPath = $"{TestingServer.BaseAddress}{PlayersRouteConstants.BaseRoute}{PlayersRouteConstants.PlayersRoute}";
            var getRequest = new HttpRequestMessage(HttpMethod.Get, requestPath);

            return HttpClient.SendAsync(getRequest);
        }
        
        protected Task<HttpResponseMessage> GetPlayerByUsernameAsync(string username)
        {
            var requestPath = $"{TestingServer.BaseAddress}{PlayersRouteConstants.BaseRoute}{PlayersRouteConstants.PlayersRoute}/{username}";
            var getRequest = new HttpRequestMessage(HttpMethod.Get, requestPath);

            return HttpClient.SendAsync(getRequest);
        }
        
        protected Task<HttpResponseMessage> AddPlayerAsync(string username)
        {
            var requestPath = $"{TestingServer.BaseAddress}{PlayersRouteConstants.BaseRoute}{PlayersRouteConstants.PlayersRoute}";
            var postRequest = new HttpRequestMessage(HttpMethod.Post, requestPath);
            postRequest.Content = new StringContent(JsonConvert.SerializeObject(username), Encoding.UTF8, "application/json");

            return HttpClient.SendAsync(postRequest);
        }

        protected Task<HttpResponseMessage> RemovePlayerAsync(string username)
        {
            var requestPath = $"{TestingServer.BaseAddress}{PlayersRouteConstants.BaseRoute}" +
                              $"{PlayersRouteConstants.PlayersRoute}/{username}";

            var removeRequest = new HttpRequestMessage(HttpMethod.Delete, requestPath);

            return HttpClient.SendAsync(removeRequest);
        }

        protected Task<HttpResponseMessage> UpdatePlayerAsync(PlayerModel updatedPlayer)
        {
            var username = updatedPlayer.Username;
            var requestPath = $"{TestingServer.BaseAddress}{PlayersRouteConstants.BaseRoute}{PlayersRouteConstants.PlayersRoute}/{username}";
            var putRequest = new HttpRequestMessage(HttpMethod.Put, requestPath);
            putRequest.Content = new StringContent(JsonConvert.SerializeObject(updatedPlayer), Encoding.UTF8, "application/json");

            return HttpClient.SendAsync(putRequest);
        }
    }
}