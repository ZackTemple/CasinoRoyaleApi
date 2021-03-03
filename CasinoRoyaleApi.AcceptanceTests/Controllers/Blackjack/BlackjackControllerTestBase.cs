using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Casino_Royale_Api.Constants;
using Casino_Royale_Api.Entities;
using CasinoRoyaleApi.AcceptanceTests.Helpers;
using Newtonsoft.Json;

namespace CasinoRoyaleApi.AcceptanceTests.Controllers.Blackjack
{
    public class BlackjackControllerTestBase : TestBase
    {
        private readonly Fixture _fixture = new Fixture();
        protected IEnumerable<Player> Players;
        
        protected BlackjackControllerTestBase(TestFixture testFixture) : base(testFixture)
        {
        }
        
        public async Task<CasinoTable> DeserializeResponse(HttpResponseMessage message)
        {
            var messageString = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CasinoTable>(messageString);
        }

        protected Task<HttpResponseMessage> StartBlackjackGame(PlayerBetViewModel model)
        {
            var requestPath = $"{TestingServer.BaseAddress}{BlackjackRouteConstants.BaseRoute}{BlackjackRouteConstants.StartGameRoute}";
            var postRequest = new HttpRequestMessage(HttpMethod.Post, requestPath);
            postRequest.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            
            return HttpClient.SendAsync(postRequest);
        }

        protected Task<HttpResponseMessage> DealCardToPlayer(CasinoTable table)
        {
            var requestPath = $"{TestingServer.BaseAddress}{BlackjackRouteConstants.BaseRoute}{BlackjackRouteConstants.PlayerHitRoute}";
            var postRequest = new HttpRequestMessage(HttpMethod.Post, requestPath);
            postRequest.Content = new StringContent(JsonConvert.SerializeObject(table), Encoding.UTF8, "application/json");

            return HttpClient.SendAsync(postRequest);
        }
        
        protected Task<HttpResponseMessage> FinishGame(CasinoTable table)
        {
            var requestPath = $"{TestingServer.BaseAddress}{BlackjackRouteConstants.BaseRoute}{BlackjackRouteConstants.PlayerStayRoute}";
            var postRequest = new HttpRequestMessage(HttpMethod.Post, requestPath);
            postRequest.Content = new StringContent(JsonConvert.SerializeObject(table), Encoding.UTF8, "application/json");

            return HttpClient.SendAsync(postRequest);
        }
    }
}