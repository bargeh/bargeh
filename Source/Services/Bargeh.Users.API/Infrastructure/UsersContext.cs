using Bargeh.Users.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.API.Infrastructure;

public class UsersContext (DbContextOptions<UsersContext> options) : DbContext (options)
{
	#region Database Objects

	public DbSet<User> Users { get; set; }

	#endregion

	#region Compiled Queries' Functions

	private static Func<UsersContext, string, Task<User?>> _getUserByUsername =
		EF.CompileAsyncQuery (
			(UsersContext context, string username) =>
				context.Users.FirstOrDefault (u => u.Username == username));

	#endregion

	#region Compiled Queries' Methods

	public async Task<User?> GetUserByUsernameAsync (string username)
	{
		return await _getUserByUsername (this, username);
	}

	#endregion
}