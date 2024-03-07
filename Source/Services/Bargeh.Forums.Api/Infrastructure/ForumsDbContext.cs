using Bargeh.Forums.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Bargeh.Forums.Api.Infrastructure;

public class ForumsDbContext(DbContextOptions<ForumsDbContext> options) : DbContext(options)
{
	#region Database Objects

	public DbSet<Forum> Forums { get; init; }
	public DbSet<ForumMembership> ForumMemberships { get; init; }
	
	#endregion

	/*#region Compiled Queries' Functions

	private static readonly Func<ForumsDbContext, string, Task<RefreshToken?>> GetRefreshTokenByOldTokenQuery =
		EF.CompileAsyncQuery(
							 (ForumsDbContext context, string oldToken) =>
								 context.RefreshTokens.FirstOrDefault(r => r.Token == oldToken));

	#endregion

	#region Compiled Queries' Methods

	public async Task<RefreshToken?> GetRefreshTokenByOldToken(string oldToken)
	{
		return await GetRefreshTokenByOldTokenQuery(this, oldToken);
	}

	#endregion*/
}