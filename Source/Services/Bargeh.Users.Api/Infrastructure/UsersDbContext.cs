using Bargeh.Users.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
	#region Database Objects

	public DbSet<User> Users { get; init; }
	public DbSet<RefreshToken> RefreshTokens { get; init; }
	public DbSet<SmsVerification> SmsVerifications { get; init; }

	#endregion

	#region Compiled Queries' Functions
	
	private static readonly Func<UsersDbContext, ushort, string, Task<SmsVerification?>> GetVerificationByCodeQuery =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, ushort code, string phone) =>
								 context.SmsVerifications.FirstOrDefault(s => s.Code == code && s.Phone == phone));
	
	private static readonly Func<UsersDbContext, string, Task<RefreshToken?>> GetRefreshTokenByOldTokenQuery =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, string oldToken) =>
								 context.RefreshTokens.FirstOrDefault(r => r.Token == oldToken));

	private static readonly Func<UsersDbContext, string, Task<User?>> GetUserByUsername =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, string username) =>
								 context.Users.FirstOrDefault(u => u.Username == username));

	private static readonly Func<UsersDbContext, string, Task<User?>> GetUserByPhone =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, string phone) =>
								 context.Users.FirstOrDefault(u => u.PhoneNumber == phone));

	private static readonly Func<UsersDbContext, Guid, Task<User?>> GetUserById =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, Guid id) =>
								 context.Users.FirstOrDefault(u => u.Id == id));

	private static readonly Func<UsersDbContext, string, string, Task<User?>> GetUserByPhoneAndPassword =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, string phone, string password) =>
								 context.Users.FirstOrDefault(u => u.PhoneNumber == phone && u.Password == password));

	private static readonly Func<UsersDbContext, string, Task<bool>> UserExistsByPhone =
		EF.CompileAsyncQuery(
							 (UsersDbContext context, string phone) =>
								 context.Users.Any(u => u.PhoneNumber == phone));

	#endregion

	#region Compiled Queries' Methods
	
	public async Task<SmsVerification?> GetVerificationByCode(ushort code, string phone)
	{
		return await GetVerificationByCodeQuery(this, code, phone);
	}
	
	public async Task<RefreshToken?> GetRefreshTokenByOldToken(string oldToken)
	{
		return await GetRefreshTokenByOldTokenQuery(this, oldToken);
	}

	public async Task<User?> GetUserByUsernameAsync(string username)
	{
		return await GetUserByUsername(this, username);
	}

	public async Task<User?> GetUserByPhoneNumberAsync(string phone)
	{
		return await GetUserByPhone(this, phone);
	}

	public async Task<User?> GetUserByIdAsync(string? id)
	{
		if(string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guid))
		{
			return null;
		}

		return await GetUserById(this, guid);
	}

	public async Task<User?> GetUserByPhoneAndPasswordAsync(string phone, string password)
	{
		password = password.Hash(HashType.SHA256);
		return await GetUserByPhoneAndPassword(this, phone, password);
	}

	public async Task<bool> UserExistsByPhoneAsync(string phone)
	{
		return await UserExistsByPhone(this, phone);
	}

	#endregion
}
