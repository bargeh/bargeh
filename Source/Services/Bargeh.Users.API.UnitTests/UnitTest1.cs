using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Bargeh.Users.API.Services;
using Microsoft.EntityFrameworkCore;
using Users.API;

namespace Bargeh.Users.API.UnitTests;

public class UnitTest1
{
	[Fact]
	public void GetUserByUsername_ReturnsCorrectUser ()
	{
		// Arrange
		string connectionString = "Connection string";

		DbContextOptionsBuilder<UsersContext> optionsBuilder = new();
		optionsBuilder.UseNpgsql (connectionString);


		UsersContext context = new(optionsBuilder.Options);


		UserService userService = new(context);

		// Act

		// Assert

	}
}