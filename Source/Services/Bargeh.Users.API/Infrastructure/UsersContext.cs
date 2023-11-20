using Microsoft.EntityFrameworkCore;
using Users.API.Models;

namespace Users.API.Infrastructure;

public class UsersContext : DbContext
{
	public UsersContext (DbContextOptions<UsersContext> options) : base (options)
	{
	}

	public DbSet<User> Users { get; set; }
}