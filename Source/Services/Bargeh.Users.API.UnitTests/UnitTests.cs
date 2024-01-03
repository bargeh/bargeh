using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Bargeh.Users.API.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Npgsql;
using Users.API;
using Xunit.Abstractions;

namespace Bargeh.Users.API.UnitTests;

public class UnitTests : IDisposable
{
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly UsersContext _context;
	private readonly UnitTestsDbProvider _dbProvider = new ();
	private readonly UserService _userService;
	private readonly string _connectionString;
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

	public UnitTests (ITestOutputHelper testOutputHelper)
	{
		//FROMHERE: Test if async helps
		_testOutputHelper = testOutputHelper;
		_connectionString = _dbProvider.PreparePostgresDb ().Result;
		DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
		optionsBuilder.UseNpgsql (_connectionString);
		_context = new (optionsBuilder.Options);
		UsersDbInitializer.InitializeDbAsync (_context, new Logger<UnitTests> (new NullLoggerFactory ())).Wait ();
		_userService = new (_context);

		if (_context.Users.Any ())
		{
			return;
		}

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
		_context.SaveChanges ();
	}

	[Fact]
	public void GetUserByUsername_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = _userService.GetUserByUsername (new ()
		{
			Username = "test"
		}, _callContext).Result;

		// Assert
		Assert.Equal ("test", user.Username);
	}

	[Fact]
	public void GetUserByUsername_ThrowsIfUserIsNotFound ()
	{
		// Act & Assert
		Assert.ThrowsAsync<RpcException> (async () =>
		{
			await _userService.GetUserByUsername (new ()
			{
				Username = "haha"
			}, _callContext);
		}).Wait ();
	}

	[Fact]
	public void GetUserByPhone_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = _userService.GetUserByPhone (new ()
		{
			Phone = "09123456789"
		}, _callContext).Result;

		// Assert
		Assert.Equal ("test", user.Username);
	}

	[Fact]
	public void GetUserByPhone_ThrowsIfUserIsNotFound ()
	{
		// Act & Assert
		Assert.ThrowsAsync<RpcException> (async () =>
		{
			await _userService.GetUserByPhone (new ()
			{
				Phone = "09112345678"
			}, _callContext);
		}).Wait ();
	}

	[Fact]
	public void GetUserByPhoneAndPassword_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = _userService.GetUserByPhoneAndPassword (new ()
		{
			Phone = "09123456789",
			Password = "5",
			Captcha = "556565"
		}, _callContext).Result;

		// Assert
		Assert.Equal ("test", user.Username);
	}

	[Fact]
	public void GetUserByPhoneAndPassword_ThrowsIfUserIsNotFound ()
	{
		// Act & Assert
		Assert.ThrowsAsync<RpcException> (async () =>
		{
			await _userService.GetUserByPhoneAndPassword (new ()
			{
				Phone = "09120000000",
				Password = "10",
				Captcha = "5"
			}, _callContext);
		}).Wait ();
	}

	[Fact]
	public void GetUserByPhoneAndPassword_ThrowsIfUserIsDisabled ()
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
		Assert.ThrowsAsync<RpcException> (async () =>
		{
			await _userService.GetUserByPhoneAndPassword (new ()
			{
				Phone = "09121212121",
				Password = "5",
				Captcha = "5"
			},
				_callContext);
		}).Wait ();
	}

	[Fact]
	public void GetUserById_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = _userService.GetUserById (new ()
		{
			Id = "9844fd47-3236-46cb-898d-607b5c5563c1"
		}, _callContext).Result;

		// Assert
		Assert.Equal ("test", user.Username);
	}

	[Fact]
	public void GetUserById_ThrowsIfUserIsNotFound ()
	{
		// Act & Assert
		Assert.ThrowsAsync<RpcException> (async () =>
		{
			await _userService.GetUserById (new ()
			{
				Id = "9844fd47-3236-46cb-898d-607b5c5560c1"
			}, _callContext);
		}).Wait ();
	}

	public void Dispose ()
	{
		
	}
}