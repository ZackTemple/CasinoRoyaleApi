using System;
using System.Collections.Generic;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;
using Casino_Royale_Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Casino_Royale_Api.Controllers
{
    [ApiController]
    [Route("api/blackjack")]
    public class BlackjackController : ControllerBase
    {
        private readonly IGameManager _gameManager;

        public BlackjackController(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        [HttpPost("start-game")]
        public ActionResult StartBlackjackGame(PlayerBetViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Error starting new game. ViewModel cannot be null.");
                }
            
                if (model.Player == null)
                {
                    return BadRequest("Error starting new game. Player cannot be null.");
                }

                if (model.Bet > model.Player.CurrentMoney || model.Bet <= 0)
                {
                    return BadRequest(
                        "Invalid bet. Please choose a bet greater than zero and less than the player's available money."
                    );
                }

                CasinoTable table = _gameManager.StartNewGame(model);
                table.Player.Cards = _gameManager.HandleAces(table.Player);

                return Ok(table);
            } 
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed when retrieving Players data.");
            }
        }

        [HttpPost("player/hit")]
        public IActionResult DealCardToPlayer(CasinoTable table)
        {
            try
            {
                if (table == null)
                {
                    return BadRequest("CasinoTable cannot be null.");
                }
                if (table.Player == null)
                {
                    return BadRequest("A player is needed in order to deal a card.");
                }

                table.Player.Cards.Add(_gameManager.DealNewCard(table));

                table.Player.Cards = _gameManager.HandleAces(table.Player);
                table.Player.Score = _gameManager.CalculateScore(table.Player.Cards);
            
                if (table.Player.Score > 21)
                {
                    return Ok(_gameManager.EndGameFromUserBust(table));
                }
            
                return Ok(table);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed when retrieving Players data.");
            }
        }

        [HttpPost("player/stay")]
        public ActionResult FinishGame(CasinoTable table)
        {
            try
            {
                if (table == null)
                {
                    return BadRequest("Casino Table cannot be null.");
                }
                
                table = _gameManager.FinishGame(table);

                return Ok(table);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed when retrieving Players data.");
            }
        }
    }
}