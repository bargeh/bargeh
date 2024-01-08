using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Bargeh.Users.API.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Users.API;

namespace Bargeh.Users.API.Tests;

public class UsersApiTests : IAsyncLifetime
{
    #region Valiables

    private const string VALID_USER_ID = "9844fd47-3236-46cb-898d-607b5c5560c1";
    private readonly TestsDbProvider _dbProvider = new ();
    private UsersContext _context = null!;
    private UserService _userService = null!;
    private string _connectionString = null!;
    private readonly ServerCallContext _callContext = TestServerCallContext.Create (
        "testMethod",
        null,
        DateTime.UtcNow,
        [],
        CancellationToken.None,
        "127.0.0.1",
        null,
        null,
        _ => Task.CompletedTask,
        () => new (),
        _ => { });

    #endregion

    public async Task InitializeAsync ()
    {
        _connectionString = await _dbProvider.PreparePostgresDb ();
        DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
        optionsBuilder.UseNpgsql (_connectionString);
        _context = new (optionsBuilder.Options);
        await UsersDbInitializer.InitializeDbAsync (_context, new Logger<UsersApiTests> (new NullLoggerFactory ()));
        _userService = new (_context);

        if (await _context.Users.AnyAsync ())
        {
            return;
        }

        User user = new ()
        {
            Id = new (VALID_USER_ID),
            Username = "test",
            DisplayName = "test display name",
            Email = "test@gmail.bargeh",
            VerificationCode = "0",
            Password = "5".Hash (HashType.SHA256),
            PhoneNumber = "09123456789"
        };

        _context.Add (user);
        await _context.SaveChangesAsync ();
    }

    public Task DisposeAsync ()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await _userService.GetUserByUsername (new ()
        {
            Username = "test"
        }, _callContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.GetUserByUsername (new ()
            {
                Username = "haha"
            }, _callContext);
        });
    }

    [Fact]
    public async Task GetUserByPhone_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await _userService.GetUserByPhone (new ()
        {
            Phone = "09123456789"
        }, _callContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByPhone_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.GetUserByPhone (new ()
            {
                Phone = "09112345678"
            }, _callContext);
        });
    }

    [Fact]
    public async Task GetUserByPhoneAndPassword_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await _userService.GetUserByPhoneAndPassword (new ()
        {
            Phone = "09123456789",
            Password = "5",
            Captcha = "556565"
        }, _callContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.GetUserByPhoneAndPassword (new ()
            {
                Phone = "09120000000",
                Password = "10",
                Captcha = "5"
            }, _callContext);
        });
    }

    [Fact]
    public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsDisabled ()
    {
        // Arrange
        User user = new ()
        {
            Id = new ("8844fd47-3236-46cb-898d-607b0c5563c1"),
            Username = "disabled",
            DisplayName = "test disabled name",
            Email = "test@gmail.disabled",
            VerificationCode = "disabled",
            Password = "5".Hash (HashType.SHA256),
            PhoneNumber = "09121212121",
            Enabled = false
        };

        _context.Add (user);
        _context.SaveChanges ();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.GetUserByPhoneAndPassword (new ()
            {
                Phone = "09121212121",
                Password = "5",
                Captcha = "5"
            },
                _callContext);
        });
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await _userService.GetUserById (new ()
        {
            Id = VALID_USER_ID
        }, _callContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserById_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.GetUserById (new ()
            {
                Id = "9844fd47-3236-46cb-898d-607b5c5sl0c1"
            },
                _callContext);
        });
    }

    [Fact]
    public async Task SetUserPassword_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.SetUserPassword (new ()
            {
                Id = "9844fd47-3236-46cb-898d-60s35c5560f1",
                Password = "blah blah blah"
            },
                _callContext);
        });
    }

    [Fact]
    public async Task SetUserPassword_SetsUsersPassword ()
    {
        // Act
        _userService.SetUserPassword (new ()
        {
            Id = VALID_USER_ID,
            Password = "blah blah blah"
        },
            _callContext);
    }

    [Fact]
    public async Task AddUser_ThrowsIfUserExists ()
    {
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.AddUser (new ()
            {
                Phone = "09123456789",
                Captcha = "1",
                Password = "pwd"
            },
                _callContext);
        });

    }

    [Fact]
    public async Task AddUser_AddsUser ()
    {
        await _userService.AddUser (new ()
        {
            Phone = "09001234567",
            Captcha = "1",
            Password = "pwd"
        },
            _callContext);
    }

    [Fact]
    public async Task DisableUser_ThrowsIfUserIsNotFound ()
    {
        // Act and Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await _userService.DisableUser (new ()
            {
                Id = VALID_USER_ID.Replace ('0', '1')
            },
                 _callContext);
        });
    }

    [Fact]
    public async Task DisableUser_DisablesUser ()
    {
        // Act
        await _userService.DisableUser (new ()
        {
            Id = VALID_USER_ID
        },
            _callContext);
    }
}