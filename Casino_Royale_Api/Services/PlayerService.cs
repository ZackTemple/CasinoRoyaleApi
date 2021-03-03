using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Casino_Royale_Api.Data;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Services
{
    public class PlayerService: IPlayerService
    {
        public readonly CasinoDbContext _context;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(CasinoDbContext context, ILogger<PlayerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            _logger.LogInformation($"Getting all Players");

            return await _context.Players.ToListAsync();
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            _logger.LogInformation($"Getting player with Id {id}");

            return await _context.Players.FirstAsync(player => player.Id == id);
        }

        public async Task<Player> GetPlayerByUsernameAsync(string username)
        {
            _logger.LogInformation($"Getting player {username}");

            return await _context.Players.FirstOrDefaultAsync(player => player.Username == username);
        }

        public async Task<Player> AddPlayerAsync(string username)
        {
            _logger.LogInformation($"Attempting to add player {username}");

            var exists = await CheckIfPlayerExistsAsync(username);
            if (exists) throw new InvalidOperationException($"{username} already exists");

            var defaultPlayer = setDefaultPlayerValues(username);

            _context.Players.Add(defaultPlayer);
            
            if (await _context.SaveChangesAsync() > 0)
            {
                return defaultPlayer;
            }
            else
            {
                throw new Exception("Failed to save to the database");
            }
        }

        private async Task<bool> CheckIfPlayerExistsAsync(string name)
        {
            return await _context.Players.FirstOrDefaultAsync(player => player.Username == name) != null;
        }

        private Player setDefaultPlayerValues(string username)
        {
            var defaultCurrentMoney = 100;
            var defaultTotalEarned = 0;
            var defaultTotalLost = 0;
            var defaultActive = false;
            
            var defaultPlayer = new Player(username, defaultCurrentMoney, defaultTotalEarned, defaultTotalLost, defaultActive);
            defaultPlayer.LastUpdated = DateTime.Now;

            return defaultPlayer;
        }

        public async Task<Player> RemovePlayerAsync(Player player)
        {
            _logger.LogInformation($"Deleting Player with id {player.Username}");

            _context.Players.Remove(player);

            if (await _context.SaveChangesAsync() > 0) return player;
            else throw new Exception($"Failed to remove {player.Username} from the database");
        }

        // model is the dto
        public async Task<Player> UpdatePlayerAsync(Player entity, PlayerModel model)
        {
            _logger.LogInformation($"Updating Player {entity.Username}");

            entity.Username = model.Username == null ? entity.Username : model.Username;
            entity.CurrentMoney = model.CurrentMoney == null ? entity.CurrentMoney : (double)model.CurrentMoney;
            entity.TotalEarned = model.TotalEarned == null ? entity.TotalEarned : (double)model.TotalEarned;
            entity.TotalLost = model.TotalLost == null ? entity.TotalLost : (double)model.TotalLost;
            entity.Active = model.Active == null ? entity.Active : model.Active;
            entity.LastUpdated = DateTime.Now;

            if (await _context.SaveChangesAsync() > 0) return entity;
            else throw new Exception($"Failed to update {entity.Username} to the database");
        }
    }
}
