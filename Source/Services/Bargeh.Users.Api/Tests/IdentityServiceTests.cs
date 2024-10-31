using Xunit;
using Moq;
using Bargeh.Users.Api.Services;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Infrastructure.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Bargeh.Users.Api.Tests
{
    public class IdentityServiceTests
    {
        private readonly Mock<UsersDbContext> _dbContextMock;
        private readonly Mock<ILogger<UsersService>> _loggerMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly IdentityService _identityService;

        public IdentityServiceTests()
        {
            _dbContextMock = new Mock<UsersDbContext>();
            _loggerMock = new Mock<ILogger<UsersService>>();
            _timeProviderMock = new Mock<TimeProvider>();
            _identityService = new IdentityService(_dbContextMock.Object, _timeProviderMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Phone = "09123456789",
                Password = "password",
                Captcha = "captcha"
            };

            var user = new ProtoUser
            {
                Id = Guid.NewGuid().ToString(),
                Phone = request.Phone,
                Email = "test@example.com",
                DisplayName = "Test User",
                Username = "testuser",
                PremiumDaysLeft = 10,
                Avatar = "avatar"
            };

            _dbContextMock.Setup(db => db.GetUserByPhoneAndPasswordAsync(request.Phone, request.Password))
                .ReturnsAsync(user);

            // Act
            var response = await _identityService.Login(request, null);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ValidRefreshToken_ReturnsTokenResponse()
        {
            // Arrange
            var request = new RefreshRequest
            {
                OldRefreshToken = "old_refresh_token"
            };

            var oldRefreshToken = new RefreshToken
            {
                Token = request.OldRefreshToken,
                UserId = Guid.NewGuid(),
                ExpireDate = DateTime.UtcNow.AddMinutes(5)
            };

            var user = new ProtoUser
            {
                Id = oldRefreshToken.UserId.ToString(),
                Phone = "09123456789",
                Email = "test@example.com",
                DisplayName = "Test User",
                Username = "testuser",
                PremiumDaysLeft = 10,
                Avatar = "avatar"
            };

            _dbContextMock.Setup(db => db.GetRefreshTokenByOldToken(request.OldRefreshToken))
                .ReturnsAsync(oldRefreshToken);

            _dbContextMock.Setup(db => db.GetUserByIdAsync(oldRefreshToken.UserId.ToString()))
                .ReturnsAsync(user);

            // Act
            var response = await _identityService.Refresh(request, null);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
        }
    }
}
