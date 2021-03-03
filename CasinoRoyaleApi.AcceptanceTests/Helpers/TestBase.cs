using System;
using System.Net.Http;
using AutoFixture;
using Casino_Royale_Api;
using Casino_Royale_Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace CasinoRoyaleApi.AcceptanceTests.Helpers
{
    public class TestBase
    {
        private static Fixture _fixture;
        public const string TestCollectionName = "PlayersCollection";
        protected TestServer TestingServer { get; }
        protected HttpClient HttpClient { get; }
        private IWebHostBuilder HostBuilder { get; }
        protected CasinoDbContext DbContext { get; }

        static TestBase()
        {
            _fixture = new Fixture();
        }

        public TestBase(TestFixture testFixture)
        {
            DbContext = testFixture.CreateDbContext();
            HostBuilder = new WebHostBuilder();
            HostBuilder.UseEnvironment("Testing").UseStartup<Startup>();
            HostBuilder.ConfigureAppConfiguration((Action<WebHostBuilderContext, IConfigurationBuilder>) 
                ((builderContext, config) => config.AddJsonFile("appsettings.json")));
            TestingServer = new TestServer(HostBuilder);
            HttpClient = TestingServer.CreateClient();
        }
    }
}