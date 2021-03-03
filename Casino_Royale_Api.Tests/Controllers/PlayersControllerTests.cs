using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Casino_Royale_Api.Constants;
using Casino_Royale_Api.Controllers;
using Casino_Royale_Api.Entities;
using Casino_Royale_Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Casino_Royale_Api.Tests.Controllers
{
    public class PlayersControllerTests
    {
        private readonly Mock<IPlayerService> _mockIPlayerService;
        private readonly PlayersController _controller;
        private readonly Fixture _fixture;

        public PlayersControllerTests()
        {
            _fixture = new Fixture();
            _mockIPlayerService = new Mock<IPlayerService>();
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("test");
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            
            _controller = new PlayersController(
                mockHttpContextAccessor.Object,
                _mockIPlayerService.Object
                );
        }
        
        
        [Fact]
        public async Task GetPlayers_ShouldCallGetAllPlayerAsyncFromService()
        {
            // Arrange
            var playersList = _fixture.CreateMany<Player>().ToList();
            _mockIPlayerService.Setup(x => x.GetAllPlayersAsync())
                .ReturnsAsync(playersList);

            // Act
            var response = await _controller.GetAllPlayers();
            var result = (OkObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x=> x.GetAllPlayersAsync(), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);
            Assert.IsType<List<PlayerModel>>(result.Value);
        }
        
        [Fact]
        public async Task GeneralExceptionThrown_Returns500ServerError()
        {
            _mockIPlayerService.Setup(x => x.GetAllPlayersAsync()).ThrowsAsync(new Exception());

            var response = await _controller.GetAllPlayers();

            var result = (ObjectResult) response.Result;
            result.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            result.Value.Should().Be(ResponseMessages.InternalServerErrorMessage);
        }
        
        

        [Fact]
        public async Task GetPlayerById_ShouldCallMethodInPlayersService()
        {
            // Arrange
            var player = _fixture.Create<Player>();
            var Id = player.Id;
            _mockIPlayerService.Setup(x => x.GetPlayerByIdAsync(Id))
                .ReturnsAsync(player);

            // Act
            var response = await _controller.GetPlayerById(Id);
            var result = (OkObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByIdAsync(Id), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);
            result.Value.Should().BeEquivalentTo((PlayerModel)player);
        }
        
        [Fact]
        public async Task GetPlayerById_ShouldReturn404IfNoPlayerFound()
        {
            // Arrange
            var player = _fixture.Create<Player>();
            var Id = player.Id;
            _mockIPlayerService.Setup(x => x.GetPlayerByIdAsync(Id))
                .ReturnsAsync((Player) null);

            // Act
            var response = await _controller.GetPlayerById(Id);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByIdAsync(Id), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetPlayerById_ShouldReturn500IfServerError()
        {
            // Arrange
            var Id = 54321;
            _mockIPlayerService.Setup(x => x.GetPlayerByIdAsync(Id)).ThrowsAsync(new Exception());

            // Act
            var response = await _controller.GetPlayerById(Id);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByIdAsync(Id), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
        
        
        
        [Fact]
        public async Task GetPlayerByUsername_ShouldCallMethodInPlayersService()
        {
            // Arrange
            var username = "KevinMalone";
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ReturnsAsync(_fixture.Build<Player>().With(x => x.Username, username).Create());

            // Act
            var response = await _controller.GetPlayerByUsername(username);
            var result = (OkObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        
        [Fact]
        private async Task GetPlayerByUsername_ShouldReturn404IfPlayerDoesNotExist()
        {
            // Arrange
            var player = _fixture.Create<Player>();
            var username = player.Username;
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ReturnsAsync((Player) null);

            // Act
            var response = await _controller.GetPlayerByUsername(username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Fact]
        private async Task GetPlayerByUsername_Returns500IfServerErrors()
        {
            // Arrange
            var player = _fixture.Create<Player>();
            var username = player.Username;
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username)).ThrowsAsync(new Exception());

            // Act
            var response = await _controller.GetPlayerByUsername(username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }



        [Fact]
        public async Task AddPlayerByModel_ShouldCreatePlayer()
        {
            // Arrange
            var playerModel = _fixture.Create<PlayerModel>();
            var createdPlayer = _fixture.Create<Player>();
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(playerModel.Username)).ReturnsAsync((Player) null);
            _mockIPlayerService.Setup(x => x.AddPlayerAsync(It.IsAny<string>())).ReturnsAsync(createdPlayer);

            // Act
            var response = await _controller.AddPlayerByUsername(playerModel.Username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.AddPlayerAsync(It.IsAny<string>()), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [Fact]
        public async Task AddPlayerByModel_ShouldReturn400IfPlayerAlreadyExists()
        {
            // Act
            var response = await _controller.AddPlayerByUsername(null);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.AddPlayerAsync(It.IsAny<string>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Value.Should().Be(ResponseMessages.PostNullObjectErrorMessage);
        }
        
        
        [Fact]
        public async Task AddPlayerByModel_ShouldReturn400IfPlayerModelIsNull()
        {
            // Arrange
            var playerModel = _fixture.Create<PlayerModel>();
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(playerModel.Username)).ReturnsAsync(new Player());
            
            // Act
            var response = await _controller.AddPlayerByUsername(playerModel.Username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.AddPlayerAsync(It.IsAny<string>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            Assert.Contains(playerModel.Username, (string) result.Value);
        }
        
        [Fact]
        public async Task AddPlayerByModel_ShouldReturn500IfExceptionThrown()
        {
            // Arrange
            var playerModel = _fixture.Create<PlayerModel>();
            _mockIPlayerService.Setup(x => x.AddPlayerAsync(It.IsAny<string>())).ThrowsAsync(new Exception());
            
            // Act
            var response = await _controller.AddPlayerByUsername(playerModel.Username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.AddPlayerAsync(It.IsAny<string>()), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }


        [Fact]
        public async Task DeletePlayer_RemovesPlayerAndReturns200()
        {
            var player = _fixture.Create<Player>();
            var username = player.Username;
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ReturnsAsync(player);

            var response = await _controller.RemovePlayer(username);
            var result = (ObjectResult) response;
            
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            _mockIPlayerService.Verify(x => x.RemovePlayerAsync(It.IsAny<Player>()), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);
            Assert.Contains(username, (string) result.Value);
        }
        
        [Fact]
        public async Task DeletePlayer_Returns400IfPlayerDoesNotExist()
        {
            var username = "MichaelScott";
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ReturnsAsync((Player) null);

            var response = await _controller.RemovePlayer(username);
            var result = (ObjectResult) response;
            
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            _mockIPlayerService.Verify(x => x.RemovePlayerAsync(It.IsAny<Player>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task DeletePlayer_Returns500ForAPIFailures()
        {
            var player = _fixture.Create<Player>();
            var username = player.Username;
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ThrowsAsync(new Exception());

            var response = await _controller.RemovePlayer(username);
            var result = (ObjectResult) response;
            
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            result.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            result.Value.Should().Be(ResponseMessages.InternalServerErrorMessage);
        }
        
        
        
        [Fact]
        public async Task UpdatePlayer_ShouldUpdateExistingPlayerAndCallService()
        {
            const string username = "KevinMalone";
            var request = _fixture.Create<PlayerModel>();
            var oldPlayer = _fixture.Create<Player>();
            
            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username))
                .ReturnsAsync(oldPlayer);

            _mockIPlayerService.Setup(x =>
                    x.UpdatePlayerAsync(oldPlayer, request))
                .ReturnsAsync(_fixture.Create<Player>());

            var response = await _controller.UpdatePlayer(request, username);
            var result = (OkObjectResult) response.Result;

            result.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task UpdatePlayer_ShouldReturn400IfUpdatedPlayerIsNull()
        {

            // Act (null here is the updatedPlayer model)
            var response = await _controller.UpdatePlayer(null, "KevinMalone");
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(It.IsAny<string>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Value.Should().Be(ResponseMessages.PutNullObjectErrorMessage);
        }
        
        [Fact]
        public async Task UpdatePlayer_ShouldReturn400IfUpdatedPlayerDoesNotExist()
        {
            var username = "JakePeralta";

            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username)).ReturnsAsync((Player) null);

            // Act (null here is the updatedPlayer model)
            var response = await _controller.UpdatePlayer(_fixture.Create<PlayerModel>(), username);
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            _mockIPlayerService.Verify(x => x.UpdatePlayerAsync(It.IsAny<Player>(), It.IsAny<PlayerModel>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task UpdatePlayer_ShouldReturn500ForInternalServerError()
        {
            var username = "JakePeralta";

            _mockIPlayerService.Setup(x => x.GetPlayerByUsernameAsync(username)).ThrowsAsync(new Exception());

            // Act (null here is the updatedPlayer model)
            var response = await _controller.UpdatePlayer(_fixture.Create<PlayerModel>(), "JakePeralta");
            var result = (ObjectResult) response.Result;
            
            // Assert
            _mockIPlayerService.Verify(x => x.GetPlayerByUsernameAsync(username), Times.Once);
            _mockIPlayerService.Verify(x => x.UpdatePlayerAsync(It.IsAny<Player>(), It.IsAny<PlayerModel>()), Times.Never);
            result.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}