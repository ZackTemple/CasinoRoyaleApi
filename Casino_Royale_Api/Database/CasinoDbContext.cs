using Microsoft.EntityFrameworkCore;
using Casino_Royale_Api.Entities;

namespace Casino_Royale_Api.Data
{
    public class CasinoDbContext : DbContext
    {
        public CasinoDbContext(DbContextOptions<CasinoDbContext> options) : base(options) {}

        public DbSet<Player> Players { get; set; }

        public void ClearPlayersDatabase()
        {
            Players.RemoveRange(Players);
            SaveChanges();
        }
    }
}
