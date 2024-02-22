using Bargeh.Tests.Shared;
using Bargeh.Users.Api.Models;
using Bargeh.Users.Api.Services;
using Grpc.Core;
using Users.Api;

namespace Bargeh.Users.Api.Tests;

public class UsersApiTests : UsersTestsBase
{
	private UsersService _userService = null!;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		_userService = new(UsersDbContext);
	}

	[Fact]
	public async Task GetUserByUsername_ReturnsCorrectUser()
	{
		// Act
		GetUserReply user = await _userService.GetUserByUsername(new()
		{
			Username = "test"
		}, CallContext);

		// Assert
		Assert.Equal("test", user.Username);
	}

	[Fact]
	public async Task GetUserByUsername_ThrowsIfUserIsNotFound()
	{
		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.GetUserByUsername(new()
			{
				Username = "haha"
			}, CallContext);
		});
	}

	[Fact]
	public async Task GetUserByPhone_ReturnsCorrectUser()
	{
		// Act
		GetUserReply user = await _userService.GetUserByPhone(new()
															  {
																  Phone = "09123456789"
															  },
															  CallContext);

		// Assert
		Assert.Equal("test", user.Username);
	}

	[Fact]
	public async Task GetUserByPhone_ThrowsIfUserIsNotFound()
	{
		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.GetUserByPhone(new()
			{
				Phone = "09112345678"
			}, CallContext);
		});
	}

	[Fact]
	public async Task GetUserByPhoneAndPassword_ReturnsCorrectUser()
	{
		// Act
		GetUserReply user = await _userService.GetUserByPhoneAndPassword(new()
		{
			Phone = "09123456789",
			Password = "5",
			Captcha = "556565"
		}, CallContext);

		// Assert
		Assert.Equal("test", user.Username);
	}

	[Fact]
	public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsNotFound()
	{
		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.GetUserByPhoneAndPassword(new()
			{
				Phone = "09120000000",
				Password = "10",
				Captcha = "5"
			}, CallContext);
		});
	}

	[Fact]
	public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsDisabled()
	{
		// Arrange
		User user = new()
		{
			Id = new("8844fd47-3236-46cb-898d-607b0c5563c1"),
			Username = "disabled",
			DisplayName = "test disabled name",
			Email = "test@gmail.disabled",
			VerificationCode = "disabled",
			Password = "5".Hash(HashType.SHA256),
			PhoneNumber = "09121212121",
			Enabled = false
		};

		UsersDbContext.Add(user);
		await UsersDbContext.SaveChangesAsync();

		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.GetUserByPhoneAndPassword(new()
														 {
															 Phone = "09121212121",
															 Password = "5",
															 Captcha = "5"
														 },
														 CallContext);
		});
	}

	[Fact]
	public async Task GetUserById_ReturnsCorrectUser()
	{
		// Act
		GetUserReply user = await _userService.GetUserById(new()
		{
			Id = VALID_USER_ID
		}, CallContext);

		// Assert
		Assert.Equal("test", user.Username);
	}

	[Fact]
	public async Task GetUserById_ThrowsIfUserIsNotFound()
	{
		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.GetUserById(new()
										   {
											   Id = "9844fd47-3236-46cb-898d-607b5c5sl0c1"
										   },
										   CallContext);
		});
	}

	[Fact]
	public async Task SetUserPassword_ThrowsIfUserIsNotFound()
	{
		// Act & Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.SetUserPassword(new()
											   {
												   Id = "9844fd47-3236-46cb-898d-60s35c5560f1",
												   Password = "blah blah blah"
											   },
											   CallContext);
		});
	}

	[Fact]
	public async Task SetUserPassword_SetsUsersPassword()
	{
		// Act
		_userService.SetUserPassword(new()
									 {
										 Id = VALID_USER_ID,
										 Password = "blah blah blah"
									 },
									 CallContext);
	}

	[Fact]
	public async Task AddUser_ThrowsIfUserExists()
	{
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.AddUser(new()
									   {
										   Phone = "09123456789",
										   Captcha = "1",
										   Password = "pwd"
									   },
									   CallContext);
		});
	}

	[Fact]
	public async Task AddUser_AddsUser()
	{
		await _userService.AddUser(new()
								   {
									   Phone = "09001234567",
									   Captcha = "1",
									   Password = "pwd"
								   },
								   CallContext);
	}

	[Fact]
	public async Task AddUser_ThrowsIfPhoneIsInvalid()
	{
		// Act and Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.AddUser(new()
									   {
										   Captcha = "54",
										   Password = "a password",
										   Phone = "not-a-phone"
									   },
									   CallContext);
		});
	}

	[Fact]
	public async Task DisableUser_ThrowsIfUserIsNotFound()
	{
		// Act and Assert
		await Assert.ThrowsAsync<RpcException>(async () =>
		{
			await _userService.DisableUser(new()
										   {
											   Id = VALID_USER_ID.Replace('0', '1')
										   },
										   CallContext);
		});
	}

	[Fact]
	public async Task DisableUser_DisablesUser()
	{
		// Act
		await _userService.DisableUser(new()
									   {
										   Id = VALID_USER_ID
									   },
									   CallContext);
	}
}