using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;
using Bargeh.Users.Api.Infrastructure.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UsersServiceTests
{
    private readonly UsersDbContext _dbContext;
    private readonly Mock<ILogger<UsersService>> _loggerMock;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: "UsersTestDb")
            .Options;

        _dbContext = new UsersDbContext(options);
        _loggerMock = new Mock<ILogger<UsersService>>();
        _usersService = new UsersService(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            DisplayName = "Test User",
            PhoneNumber = "09123456789"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var request = new GetUserByUsernameRequest { Username = "testuser" };
        var callContext = new Mock<ServerCallContext>().Object;

        // Act
        var result = await _usersService.GetUserByUsername(request, callContext);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldThrowRpcException_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new GetUserByUsernameRequest { Username = "nonexistentuser" };
        var callContext = new Mock<ServerCallContext>().Object;

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _usersService.GetUserByUsername(request, callContext));
    }

    [Fact]
    public async Task AddUser_ShouldAddUser_WhenUserIsValid()
    {
        // Arrange
        var request = new AddUserRequest
        {
            Phone = "09123456789",
            Username = "newuser",
            DisplayName = "New User",
            AcceptedTos = true
        };
        var callContext = new Mock<ServerCallContext>().Object;

        // Act
        var result = await _usersService.AddUser(request, callContext);

        // Assert
        Assert.NotNull(result);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.Phone);
        Assert.NotNull(user);
        Assert.Equal(request.Username + "1234", user.Username); // Assuming the discriminator is 1234
    }

    [Fact]
    public async Task AddUser_ShouldThrowRpcException_WhenUserAlreadyExists()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "existinguser",
            DisplayName = "Existing User",
            PhoneNumber = "09123456789"
        };
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var request = new AddUserRequest
        {
            Phone = "09123456789",
            Username = "newuser",
            DisplayName = "New User",
            AcceptedTos = true
        };
        var callContext = new Mock<ServerCallContext>().Object;

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _usersService.AddUser(request, callContext));
    }
}
