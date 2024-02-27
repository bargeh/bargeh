using Bargeh.Identity.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Identity.Api.Infrastructure;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
	#region Database Objects

	public DbSet<RefreshToken> RefreshTokens { get; init; }

	#endregion

	#region Compiled Queries' Functions

	private static readonly Func<IdentityDbContext, string, Task<RefreshToken?>> GetRefreshTokenByOldTokenQuery =
		EF.CompileAsyncQuery(
							 (IdentityDbContext context, string oldToken) =>
								 context.RefreshTokens.FirstOrDefault(r => r.Token == oldToken));

	#endregion

	#region Compiled Queries' Methods

	public async Task<RefreshToken?> GetRefreshTokenByOldToken(string oldToken)
	{
		return await GetRefreshTokenByOldTokenQuery(this, oldToken);
	}

	#endregion
}