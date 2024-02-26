using Bargeh.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public class UsersDbContext (DbContextOptions<UsersDbContext> options) : DbContext (options)
{
	#region Database Objects

	public DbSet<User> Users { get; init; }

	#endregion

	#region Compiled Queries' Functions

	private static Func<UsersDbContext, string, Task<User?>> _getUserByUsername =
		EF.CompileAsyncQuery (
			(UsersDbContext context, string username) =>
				context.Users.FirstOrDefault (u => u.Username == username));

	private static Func<UsersDbContext, string, Task<User?>> _getUserByPhone =
		EF.CompileAsyncQuery (
			(UsersDbContext context, string phone) =>
				context.Users.FirstOrDefault (u => u.PhoneNumber == phone));

	private static Func<UsersDbContext, Guid, Task<User?>> _getUserById =
		EF.CompileAsyncQuery (
			(UsersDbContext context, Guid id) =>
				context.Users.FirstOrDefault (u => u.Id == id));

	private static Func<UsersDbContext, string, string, Task<User?>> _getUserByPhoneAndPassword =
		EF.CompileAsyncQuery (
			(UsersDbContext context, string phone, string password) =>
				context.Users.FirstOrDefault (u => u.PhoneNumber == phone && u.Password == password));

	private static Func<UsersDbContext, string, Task<bool>> _userExistsByPhone =
		EF.CompileAsyncQuery (
			(UsersDbContext context, string phone) =>
				context.Users.Any (u => u.PhoneNumber == phone));

	#endregion

	#region Compiled Queries' Methods

	public async Task<User?> GetUserByUsernameAsync (string username)
	{
		return await _getUserByUsername (this, username);
	}

	public async Task<User?> GetUserByPhoneNumberAsync (string phone)
	{
		return await _getUserByPhone (this, phone);
	}

	public async Task<User?> GetUserByIdAsync (string? id)
	{
		if (string.IsNullOrWhiteSpace (id) || !Guid.TryParse (id, out Guid guid))
		{
			return null;
		}

		return await _getUserById (this, guid);
	}

	public async Task<User?> GetUserByPhoneAndPasswordAsync (string phone, string password)
	{
		password = password.Hash (HashType.SHA256);
		return await _getUserByPhoneAndPassword (this, phone, password);
	}

	public async Task<bool> UserExistsByPhoneAsync (string phone)
	{
		return await _userExistsByPhone (this, phone);
	}

	#endregion
}