using Bargeh.Identity.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Identity.Api.Infrastructure;

public class IdentityDbContext (DbContextOptions<IdentityDbContext> options) : DbContext (options)
{
	public DbSet<RefreshToken> RefreshTokens { get; set; }
}