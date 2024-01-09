using Bargeh.Tests.Shared;
using Bargeh.Users.Api.Models;
using Grpc.Core;
using Users.Api;

namespace Bargeh.Users.Api.Tests;

public class UsersApiTests : UsersTestsBase
{
    [Fact]
    public async Task GetUserByUsername_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await UserService.GetUserByUsername (new ()
        {
            Username = "test"
        }, CallContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.GetUserByUsername (new ()
            {
                Username = "haha"
            }, CallContext);
        });
    }

    [Fact]
    public async Task GetUserByPhone_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await UserService.GetUserByPhone (new ()
        {
            Phone = "09123456789"
        }, CallContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByPhone_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.GetUserByPhone (new ()
            {
                Phone = "09112345678"
            }, CallContext);
        });
    }

    [Fact]
    public async Task GetUserByPhoneAndPassword_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await UserService.GetUserByPhoneAndPassword (new ()
        {
            Phone = "09123456789",
            Password = "5",
            Captcha = "556565"
        }, CallContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.GetUserByPhoneAndPassword (new ()
            {
                Phone = "09120000000",
                Password = "10",
                Captcha = "5"
            }, CallContext);
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

        Context.Add (user);
        Context.SaveChanges ();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.GetUserByPhoneAndPassword (new ()
            {
                Phone = "09121212121",
                Password = "5",
                Captcha = "5"
            },
                CallContext);
        });
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser ()
    {
        // Act
        GetUserReply user = await UserService.GetUserById (new ()
        {
            Id = VALID_USER_ID
        }, CallContext);

        // Assert
        Assert.Equal ("test", user.Username);
    }

    [Fact]
    public async Task GetUserById_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.GetUserById (new ()
            {
                Id = "9844fd47-3236-46cb-898d-607b5c5sl0c1"
            },
                CallContext);
        });
    }

    [Fact]
    public async Task SetUserPassword_ThrowsIfUserIsNotFound ()
    {
        // Act & Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.SetUserPassword (new ()
            {
                Id = "9844fd47-3236-46cb-898d-60s35c5560f1",
                Password = "blah blah blah"
            },
                CallContext);
        });
    }

    [Fact]
    public async Task SetUserPassword_SetsUsersPassword ()
    {
        // Act
        UserService.SetUserPassword (new ()
        {
            Id = VALID_USER_ID,
            Password = "blah blah blah"
        },
            CallContext);
    }

    [Fact]
    public async Task AddUser_ThrowsIfUserExists ()
    {
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.AddUser (new ()
            {
                Phone = "09123456789",
                Captcha = "1",
                Password = "pwd"
            },
                CallContext);
        });

    }

    [Fact]
    public async Task AddUser_AddsUser ()
    {
        await UserService.AddUser (new ()
        {
            Phone = "09001234567",
            Captcha = "1",
            Password = "pwd"
        },
            CallContext);
    }

    [Fact]
    public async Task DisableUser_ThrowsIfUserIsNotFound ()
    {
        // Act and Assert
        await Assert.ThrowsAsync<RpcException> (async () =>
        {
            await UserService.DisableUser (new ()
            {
                Id = VALID_USER_ID.Replace ('0', '1')
            },
                 CallContext);
        });
    }

    [Fact]
    public async Task DisableUser_DisablesUser ()
    {
        // Act
        await UserService.DisableUser (new ()
        {
            Id = VALID_USER_ID
        },
            CallContext);
    }
}