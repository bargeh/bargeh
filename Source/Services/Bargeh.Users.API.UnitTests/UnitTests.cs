using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Bargeh.Users.API.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Users.API;
using Xunit.Abstractions;

namespace Bargeh.Users.API.UnitTests;

public class UnitTests ()
{
	[Fact]
	public async Task GetUserByUsername_ReturnsCorrectUser ()
	{
		// Arrange
		string connectionString = await UnitTestsDbProvider.PreparePostgresDb ();
		DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
		optionsBuilder.UseNpgsql (connectionString);
		UsersContext context = new (optionsBuilder.Options);
		await UsersDbInitializer.InitializeDbAsync (context, new Logger<UnitTests>(new NullLoggerFactory()));

		UserService userService = new (context);

		ServerCallContext callContext = TestServerCallContext.Create (
			"testMethod",
			null,
			DateTime.UtcNow,
			new (),
			CancellationToken.None,
			"127.0.0.1",
			null,
			null,
			m => Task.CompletedTask,
			() => new (),
			w => { });

		// Act
		GetUserReply user = await userService.GetUserByUsername(new()
		{
			Username = "test"
		}, callContext);

		// Assert
		Assert.Equal(user, user);
	}
}