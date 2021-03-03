using System.Collections.Generic;
using System.Threading.Tasks;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Services
{
    public interface IPlayerService
    {
        Task<List<Player>> GetAllPlayersAsync();
        Task<Player> GetPlayerByUsernameAsync(string username);
        Task<Player> GetPlayerByIdAsync(int id);
        Task<Player> AddPlayerAsync(string username);
        Task<Player> RemovePlayerAsync(Player player);
        Task<Player> UpdatePlayerAsync(Player entity, PlayerModel model);
    }
}
