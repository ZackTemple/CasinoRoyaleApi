using System;
using Casino_Royale_Api.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CasinoRoyaleApi.AcceptanceTests.Helpers
{
    public class TestFixture : IDisposable
    {
        public readonly SqliteConnection _connection;
        public string _connectionString;
        private CasinoDbContext _dbContext;
        
        public TestFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _connectionString = configuration.GetConnectionString("localDb"); 
            _connection = new SqliteConnection(_connectionString);
        }

        public CasinoDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<CasinoDbContext>()
                .UseSqlite(_connection)
                .Options;
            _dbContext = new CasinoDbContext(options);
            return _dbContext;
        }
        
        public void Dispose()
        {
            _connection.Close();
            _dbContext.Dispose();
        }
    }
}