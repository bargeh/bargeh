using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Users.API;

namespace Bargeh.Users.API.UnitTests;

public class UnitTests : IAsyncLifetime
{
	private UsersContext _context = default!;
	private readonly UnitTestsDbProvider _dbProvider = new ();
	private UserService _userService;
	private readonly ServerCallContext _callContext = TestServerCallContext.Create (
		"testMethod",
		null,
		DateTime.UtcNow,
		[],
		CancellationToken.None,
		"127.0.0.1",
		null,
		null,
		m => Task.CompletedTask,
		() => new (),
		w => { });

	public async Task InitializeAsync ()
	{
		string connectionString = await _dbProvider.PreparePostgresDbAsync ();
		DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
		optionsBuilder.UseNpgsql (connectionString);
		_context = new (optionsBuilder.Options);
		await UsersDbInitializer.InitializeDbAsync (_context, new Logger<UnitTests> (new NullLoggerFactory ()));
		_userService = new (_context);
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
	public async Task GetUserByPhone_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = await _userService.GetUserByPhone(new()
		{
			Phone = "09123456789"
		}, _callContext);

		// Assert
		Assert.Equal("test", user.Username);
	}

	[Fact]
	public async Task GetUserByPhoneAndPassword_ReturnsCorrectUser ()
	{
		// Act
		GetUserReply user = await _userService.GetUserByPhoneAndPassword(new()
		{
			Phone = "09123456789",
			Password = "5",
			Captcha = "556565"
		}, _callContext);

		// Assert
		Assert.Equal("test", user.Username);
	}
}