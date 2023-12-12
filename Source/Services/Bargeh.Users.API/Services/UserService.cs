using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Users.API;

namespace Bargeh.Users.API.Services;

public class UserService (UsersContext context) : UsersProto.UsersProtoBase
{
	public override async Task<GetUserReply> GetUserByUsername (GetUserByUsernameRequest request,
																ServerCallContext callContext)
	{
		var user = await context.GetUserByUsernameAsync (request.Username);

		if (user == null)
		{
			throw new RpcException (new (StatusCode.NotFound, "Not Found"));
		}

		return new ()
		{
			Id = user.Id.ToString (),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			CoverImage = user.CoverImage,
			DisplayName = user.DisplayName,
			Email = user.Email,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			ProfileImage = user.ProfileImage
		};
	}

	public override async Task<GetUserReply> GetUserByPhone (GetUserByPhoneRequest request,
															 ServerCallContext callContext)
	{
		var user = await context.Users.FirstOrDefaultAsync (u => u.PhoneNumber == request.Phone);

		if (user == null)
		{
			throw new RpcException (new (StatusCode.NotFound, "Not Found"));
		}

		return new ()
		{
			Id = user.Id.ToString (),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			CoverImage = user.CoverImage,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			ProfileImage = user.ProfileImage
		};
	}

	public override async Task<GetUserReply> GetUserById (GetUserByIdRequest request, ServerCallContext callContext)
	{
		Guid? guid;

		try
		{
			guid = Guid.Parse (request.Id);
		}
		catch (Exception)
		{
			throw new RpcException (new (StatusCode.InvalidArgument, "Invalid Argument"));
		}

		var user = await context.Users.FirstOrDefaultAsync (u => u.Id == guid);

		if (user == null)
		{
			throw new RpcException (new (StatusCode.NotFound, "Not Found"));
		}

		return new ()
		{
			Id = user.Id.ToString (),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			CoverImage = user.CoverImage,
			DisplayName = user.DisplayName,
			Email = user.Email,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			ProfileImage = user.ProfileImage
		};
	}

	public override async Task<GetUserReply> GetUserByPhoneAndPassword (GetUserByPhoneAndPasswordRequest request,
																		ServerCallContext callContext)
	{
		var user = await context.Users.FirstOrDefaultAsync (u =>
			u.PhoneNumber == request.Phone && u.Password == request.Password.Hash (HashType.SHA256));

		// TODO: Verify captcha

		if (user == null)
		{
			throw new RpcException (new (StatusCode.NotFound, "Not Found"));
		}

		if (!user.Enabled)
		{
			throw new RpcException (new (StatusCode.PermissionDenied, "Permission Denied"));
		}

		return new ()
		{
			Id = user.Id.ToString (),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			CoverImage = user.CoverImage,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			ProfileImage = user.ProfileImage
		};
	}

	public override async Task<VoidOperationReply> SetUserPassword (SetUserPasswordRequest request,
																	ServerCallContext callContext)
	{
		var user = await context.Users.FirstOrDefaultAsync (u => u.Id == Guid.Parse (request.Id)) ?? throw new RpcException (new (StatusCode.NotFound, "Not Found"));

		user.Password = request.Password;

		await context.SaveChangesAsync ();

		return new ()
		{
			Success = true
		};
	}

	public override async Task<VoidOperationReply> AddUser (AddUserRequest request, ServerCallContext callContext)
	{
		var userExists =
			await context.Users.AnyAsync (u => u.PhoneNumber == request.Phone);

		if (userExists)
		{
			throw new RpcException (new (StatusCode.AlreadyExists, "Already Exists"));
		}

		int discriminator = Random.Shared.Next (10000, 99999);

		string username = $"user{discriminator}";
		string displayName = "کاربر " + discriminator;

		User user = new ()
		{
			PhoneNumber = request.Phone,
			DisplayName = displayName,
			Username = username,
			VerificationCode = Guid.NewGuid ().ToString ().Replace ("-", ""),
			Password = request.Password.Hash (HashType.SHA256)
		};

		context.Add (user);
		await context.SaveChangesAsync ();

		return new ();
	}
}