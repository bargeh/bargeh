using Bargeh.Users.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.API.Infrastructure;

public class UsersContext : DbContext
{
	public UsersContext (DbContextOptions<UsersContext> options) : base (options)
	{
	}

	public DbSet<User> Users { get; set; }
}