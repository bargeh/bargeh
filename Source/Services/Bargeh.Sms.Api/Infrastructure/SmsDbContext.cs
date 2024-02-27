using Bargeh.Sms.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Bargeh.Sms.Api.Infrastructure;

public class SmsDbContext(DbContextOptions<SmsDbContext> options) : DbContext(options)
{
	#region Database Objects

	public DbSet<SmsVerification> SmsVerifications { get; init; }

	#endregion

	#region Compiled Queries' Functions

	private static readonly Func<SmsDbContext, ushort, string, Task<SmsVerification?>> GetVerificationByCodeQuery =
		EF.CompileAsyncQuery(
							 (SmsDbContext context, ushort code, string phone) =>
								 context.SmsVerifications.FirstOrDefault(s => s.Code == code && s.Phone == phone));

	#endregion

	#region Compiled Queries' Methods

	public async Task<SmsVerification?> GetVerificationByCode(ushort code, string phone)
	{
		return await GetVerificationByCodeQuery(this, code, phone);
	}

	#endregion
}