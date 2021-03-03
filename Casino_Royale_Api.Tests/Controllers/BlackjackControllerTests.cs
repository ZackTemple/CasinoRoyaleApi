using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoFixture;
using Casino_Royale_Api.Controllers;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Models;
using Casino_Royale_Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;
using Moq;
using Xunit;

namespace Casino_Royale_Api.Tests.Controllers
{
    public class BlackjackControllerTests
    {
        private readonly Mock<IGameManager> _mockGameManager;
        private readonly BlackjackController _controller;
        private readonly Fixture _fixture = new Fixture();

        public BlackjackControllerTests()
        {
            _mockGameManager = new Mock<IGameManager>();
            _controller = new BlackjackController(_mockGameManager.Object);
        }

        [Fact]
        public void StartGame_Returns400IfViewModelIsNull()
        {
            var playerBetViewModel = _fixture.Create<PlayerBetViewModel>();
            var response = _controller.StartBlackjackGame(null) as ObjectResult;

            _mockGameManager.Verify(x => x.StartNewGame(It.IsAny<PlayerBetViewModel>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [Fact]
        public void StartGame_Returns400IfPlayerIsNull()
        {
            var playerBetViewModel = _fixture.Create<PlayerBetViewModel>();
            playerBetViewModel.Player = null;
            var response = _controller.StartBlackjackGame(playerBetViewModel) as ObjectResult;

            _mockGameManager.Verify(x => x.StartNewGame(It.IsAny<PlayerBetViewModel>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public void StartGame_Returns400IfBetIsGreaterThanPlayersWallet()
        {
            var playerBetViewModel = _fixture.Create<PlayerBetViewModel>();
            playerBetViewModel.Bet = (double) (playerBetViewModel.Player.CurrentMoney + 1);
            
            var response = _controller.StartBlackjackGame(playerBetViewModel) as ObjectResult;

            _mockGameManager.Verify(x => x.StartNewGame(It.IsAny<PlayerBetViewModel>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public void StartGame_SetsUpNewTable()
        {
            var playerBetViewModel = _fixture.Create<PlayerBetViewModel>();
            // Setup custom values to insure that Bet is not greater than CurrentMoney
            playerBetViewModel.Player.CurrentMoney = 500;
            playerBetViewModel.Bet = 50;
            _mockGameManager.Setup(x => x.StartNewGame(playerBetViewModel)).Returns(_fixture.Create<CasinoTable>());
            
            var response = _controller.StartBlackjackGame(playerBetViewModel) as ObjectResult;
            
            _mockGameManager.Verify(x => x.StartNewGame(playerBetViewModel), Times.Once);
            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
        
        [Fact]
        public void StartGame_Returns500IfApiErrors()
        {
            var playerBetViewModel = _fixture.Create<PlayerBetViewModel>();
            playerBetViewModel.Player.CurrentMoney = 500;
            playerBetViewModel.Bet = 50;
            _mockGameManager.Setup(x => x.StartNewGame(It.IsAny<PlayerBetViewModel>()))
                .Throws(new Exception());

            var response = _controller.StartBlackjackGame(playerBetViewModel) as ObjectResult;
            
            response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        

        [Fact]
        public void DealCardToPlayer_Returns400IfTableIsNull()
        {
            var response = _controller.DealCardToPlayer(null) as ObjectResult;
            
            _mockGameManager.Verify(x => x.DealNewCard(It.IsAny<CasinoTable>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public void DealCardToPlayer_Returns400IfPlayerIsNull()
        {
            var table = _fixture.Create<CasinoTable>();
            table.Player = null;
            
            var response = _controller.DealCardToPlayer(table) as ObjectResult;
            
            _mockGameManager.Verify(x => x.DealNewCard(It.IsAny<CasinoTable>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public void DealCardToPlayer_ReturnsTableWithResultIfPlayerBusts()
        {
            var table = _fixture.Create<CasinoTable>();
            
            _mockGameManager.Setup(x => x.DealNewCard(table)).Returns(_fixture.Create<Card>());
            _mockGameManager.Setup(x => x.HandleAces(table.Player))
                .Returns(_fixture.CreateMany<Card>().ToList());
            _mockGameManager.Setup(x => x.CalculateScore(It.IsAny<List<Card>>())).Returns(22);
            _mockGameManager.Setup(x => x.EndGameFromUserBust(It.IsAny<CasinoTable>()))
                .Returns(
                    _fixture.Build<CasinoTable>().With(t => t.Result, Result.Bust.ToString()).Create()
                    );
            

            var response = _controller.DealCardToPlayer(table) as ObjectResult;
            var returnedTable = (CasinoTable) response.Value;
            
            _mockGameManager.Verify(x => x.DealNewCard(table), Times.Once);
            _mockGameManager.Verify(x => x.HandleAces(It.IsAny<CasinoPlayer>()), Times.Once);
            _mockGameManager.Verify(x => x.CalculateScore(It.IsAny<List<Card>>()), Times.Once);
            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            returnedTable.Result.Should().Be(Result.Bust.ToString());
        }
        
        [Fact]
        public void DealCardToPlayer_ReturnsTableWithNoResultIfPlayerDoesNotBust()
        {
            var table = _fixture.Create<CasinoTable>();
            
            _mockGameManager.Setup(x => x.DealNewCard(table)).Returns(_fixture.Create<Card>());
            _mockGameManager.Setup(x => x.HandleAces(table.Player))
                .Returns(_fixture.CreateMany<Card>().ToList());
            _mockGameManager.Setup(x => x.CalculateScore(It.IsAny<List<Card>>())).Returns(22);
            _mockGameManager.Setup(x => x.EndGameFromUserBust(It.IsAny<CasinoTable>()))
                .Returns(
                    _fixture.Build<CasinoTable>().With(t => t.Result, (string) null).Create()
                );
            

            var response = _controller.DealCardToPlayer(table) as ObjectResult;
            var returnedTable = (CasinoTable) response.Value;
            
            _mockGameManager.Verify(x => x.DealNewCard(table), Times.Once);
            _mockGameManager.Verify(x => x.HandleAces(It.IsAny<CasinoPlayer>()), Times.Once);
            _mockGameManager.Verify(x => x.CalculateScore(It.IsAny<List<Card>>()), Times.Once);
            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            returnedTable.Result.Should().BeNull();
        }
        
        [Fact]
        public void DealCardToPlayer_Returns500IfApiErrors()
        {
            var table = _fixture.Create<CasinoTable>();
            _mockGameManager.Setup(x => x.DealNewCard(It.IsAny<CasinoTable>()))
                .Throws(new Exception());

            var response = _controller.DealCardToPlayer(table) as ObjectResult;
            
            response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }


        [Fact]
        public void FinishGame_Returns400IfCasinoTableIsNull()
        {
            var response = _controller.FinishGame(null) as ObjectResult;
            
            _mockGameManager.Verify(x => x.FinishGame(It.IsAny<CasinoTable>()), Times.Never);
            response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public void FinishGame_ReturnsCasinoTable()
        {
            var table = _fixture.Create<CasinoTable>();
            _mockGameManager.Setup(x => x.FinishGame(table))
                .Returns(_fixture.Create<CasinoTable>());
            
            var response = _controller.FinishGame(table) as ObjectResult;
            
            _mockGameManager.Verify(x => x.FinishGame(It.IsAny<CasinoTable>()), Times.Once);
            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
        
        [Fact]
        public void FinishGame_Returns500IfApiErrors()
        {
            _mockGameManager.Setup(x => x.FinishGame(It.IsAny<CasinoTable>()))
                .Throws(new Exception());
            var response = _controller.FinishGame(_fixture.Create<CasinoTable>()) as ObjectResult;
            
            response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}