using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Bargeh.Users.API.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Users.API;

namespace Bargeh.Users.API.UnitTests;

public class UnitTests : IAsyncLifetime
{
	private UsersContext _context = default!;
	private readonly UnitTestsDbProvider _dbProvider = new ();
	private UserService _userService = null!;
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

	public async Task InitializeAsync ()
	{
		string connectionString = await _dbProvider.PreparePostgresDbAsync ();
		DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
		optionsBuilder.UseNpgsql (connectionString);
		_context = new (optionsBuilder.Options);
		await UsersDbInitializer.InitializeDbAsync (_context, new Logger<UnitTests> (new NullLoggerFactory ()));
		_userService = new (_context);

		if (!_context.Users.Any ())
		{
			User user = new ()
			{
				Id = new ("9844fd47-3236-46cb-898d-607b5c5563c1"),
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
	}

	public async Task DisposeAsync ()
	{
		await _dbProvider.DisposePostgresDbAsync ();
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

	//[Fact]
	//public async Task GetUserByPhone_ThrowsIfUserIsNotFound ()
	//{
	//	// Act & Assert
	//	await Assert.ThrowsAsync<RpcException> (async () =>
	//	{
	//		await _userService.GetUserByPhone (new ()
	//		{
	//			Phone = "09112345678"
	//		}, _callContext);
	//	});
	//}

	//[Fact]
	//public async Task GetUserByPhoneAndPassword_ReturnsCorrectUser ()
	//{
	//	// Act
	//	GetUserReply user = await _userService.GetUserByPhoneAndPassword (new ()
	//	{
	//		Phone = "09123456789",
	//		Password = "5",
	//		Captcha = "556565"
	//	}, _callContext);

	//	// Assert
	//	Assert.Equal ("test", user.Username);
	//}

	//[Fact]
	//public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsNotFound ()
	//{
	//	// Act & Assert
	//	await Assert.ThrowsAsync<RpcException> (async () =>
	//	{
	//		await _userService.GetUserByPhoneAndPassword (new ()
	//		{
	//			Phone = "09120000000",
	//			Password = "10",
	//			Captcha = "5"
	//		}, _callContext);
	//	});
	//}

	//[Fact]
	//public async Task GetUserByPhoneAndPassword_ThrowsIfUserIsDisabled()
	//{
	//	// Arrange
	//	User user = new ()
	//	{
	//		Id = new ("8844fd47-3236-46cb-898d-607b5c5563c1"),
	//		Username = "disabled",
	//		DisplayName = "test disabled name",
	//		Email = "test@gmail.disabled",
	//		VerificationCode = "disabled",
	//		Password = "5".Hash (HashType.SHA256),
	//		PhoneNumber = "09121212121",
	//		Enabled = false
	//	};

	//	_context.Add (user);
	//	await _context.SaveChangesAsync ();

	//	// Act & Assert
	//	await Assert.ThrowsAsync<RpcException> (async () =>
	//	{
	//		await _userService.GetUserByPhoneAndPassword (new ()
	//		{
	//			Phone = "09121212121",
	//			Password = "5",
	//			Captcha = "5"
	//		}, 
	//			_callContext);
	//	});
	//}

	//[Fact]
	//public async Task GetUserById_ReturnsCorrectUser ()
	//{
	//	// Act
	//	GetUserReply user = await _userService.GetUserById (new ()
	//	{
	//		Id = "9844fd47-3236-46cb-898d-607b5c5563c1"
	//	}, _callContext);

	//	// Assert
	//	Assert.Equal ("test", user.Username);
	//}

	//[Fact]
	//public async Task GetUserById_ThrowsIfUserIsNotFound ()
	//{
	//	// Act & Assert
	//	await Assert.ThrowsAsync<RpcException> (async () =>
	//	{
	//		await _userService.GetUserById(new ()
	//		{
	//			Id = "9844fd47-3236-46cb-898d-607b5c5560c1"
	//		}, _callContext);
	//	});
	//}
}