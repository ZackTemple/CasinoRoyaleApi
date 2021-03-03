using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Casino_Royale_Api.Constants;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;
using Casino_Royale_Api.Services;


namespace Casino_Royale_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        
        private readonly IHttpContextAccessor _http;
        private readonly IPlayerService _service;
        public PlayersController(IHttpContextAccessor http, IPlayerService service)
        {
            _http = http;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlayerModel>>> GetAllPlayers()
        {
            try
            {
                var players = await _service.GetAllPlayersAsync();

                var playerModels = players.Select(p => (PlayerModel) p).ToList();

                return Ok(playerModels);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlayerModel>> GetPlayerById([FromRoute] int id)
        {
            try
            {
                Player player = await _service.GetPlayerByIdAsync(id);

                if (player == null) return NotFound($"Player with {id} does not exist.");
                PlayerModel playerModel = (PlayerModel)player;

                return Ok(playerModel);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"No player with id '{id}' is in the database");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<PlayerModel>> GetPlayerByUsername([FromRoute] string username)
        {
            try
            {
                Player player = await _service.GetPlayerByUsernameAsync(username);

                if (player == null) return NotFound($"Player {username} does not exist.");
                PlayerModel playerModel = (PlayerModel)player;

                return Ok(playerModel);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerModel>> AddPlayerByUsername([FromBody] string username)
        {
            if (username == null)
            {
                return BadRequest(ResponseMessages.PostNullObjectErrorMessage);
            }
            try
            {
                // Check to see if player already exists
                var playerToCreate = await _service.GetPlayerByUsernameAsync(username);
                if (playerToCreate != null) return BadRequest($"Player {username} already exists.");

                var playerAdded = await _service.AddPlayerAsync(username);

                var uri = _http.HttpContext.Request.Host.Value;
                return Created(uri, (PlayerModel)playerAdded);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult> RemovePlayer([FromRoute] string username) {
            try
            {
                var oldPlayer = await _service.GetPlayerByUsernameAsync(username);
                if (oldPlayer == null) return BadRequest(ResponseMessages.PlayerDoesNotExistMessage(username));

                await _service.RemovePlayerAsync(oldPlayer);
                return Ok($"Player '{oldPlayer.Username}' was removed from the database");

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }
        }

        [HttpPut("{username}")]
        public async Task<ActionResult<PlayerModel>> UpdatePlayer([FromBody] PlayerModel dto, [FromRoute] string username)
        {
            if (dto == null)
            {
                return BadRequest(ResponseMessages.PutNullObjectErrorMessage);
            }
            try
            {
                Player player = await _service.GetPlayerByUsernameAsync(username);
                if (player == null)
                {
                    return BadRequest(ResponseMessages.PlayerDoesNotExistMessage(username));
                }
                
                player = await _service.UpdatePlayerAsync(player, dto);
                PlayerModel playerModel = (PlayerModel)player;
                return Ok(playerModel);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ResponseMessages.InternalServerErrorMessage);
            }
        }
    }
}
