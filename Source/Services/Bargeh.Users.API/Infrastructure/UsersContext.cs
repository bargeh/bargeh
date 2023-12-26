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

	private static Func<UsersContext, string, Task<User?>> _getUserByPhone =
		EF.CompileAsyncQuery (
			(UsersContext context, string phone) =>
				context.Users.FirstOrDefault (u => u.PhoneNumber == phone));

	private static Func<UsersContext, Guid, Task<User?>> _getUserById =
		EF.CompileAsyncQuery (
			(UsersContext context, Guid id) =>
				context.Users.FirstOrDefault (u => u.Id == id));

	private static Func<UsersContext, string, string, Task<User?>> _getUserByPhoneAndPassword =
		EF.CompileAsyncQuery (
			(UsersContext context, string phone, string password) =>
				context.Users.FirstOrDefault (u => u.PhoneNumber == phone && u.Password == password));

	private static Func<UsersContext, string, Task<bool>> _userExistsByPhone =
		EF.CompileAsyncQuery (
			(UsersContext context, string phone) =>
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