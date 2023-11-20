using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Users.API.Infrastructure;
using Users.API.Models;

namespace Users.API.Services;

public class UserService : UsersProto.UsersProtoBase
{
	private readonly UsersContext _context;

	public UserService (ILogger<UserService> logger, UsersContext context)
	{
		_context = context;
	}

	public override async Task<GetUserReply> GetUserByUsername (GetUserByUsernameRequest request,
	                                                            ServerCallContext context)
	{
		Metadata metadata = new ();
		var user = await _context.Users.FirstOrDefaultAsync (u => u.Username == request.Username);

		if (user == null)
		{
			throw new RpcException (new Status (StatusCode.NotFound, "Not Found"), metadata);
		}

		return new GetUserReply
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

	public override async Task<GetUserReply> GetUserByPhone (GetUserByPhoneRequest request, ServerCallContext context)
	{
		Metadata metadata = new ();
		var user = await _context.Users.FirstOrDefaultAsync (u => u.PhoneNumber == request.Phone);

		if (user == null)
		{
			throw new RpcException (new Status (StatusCode.NotFound, "Not Found"), metadata);
		}

		return new GetUserReply
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

	public override async Task<GetUserReply> GetUserById (GetUserByIdRequest request, ServerCallContext context)
	{
		Metadata metadata = new ();
		Guid? guid;

		try
		{
			guid = Guid.Parse (request.Id);
		}
		catch (Exception)
		{
			throw new RpcException (new Status (StatusCode.InvalidArgument, "Invalid Argument"), metadata);
		}

		var user = await _context.Users.FirstOrDefaultAsync (u => u.Id == guid);

		if (user == null)
		{
			throw new RpcException (new Status (StatusCode.NotFound, "Not Found"), metadata);
		}

		return new GetUserReply
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
	                                                                    ServerCallContext context)
	{
		//TODO: Fix null user
		Metadata metadata = new ();

		var user = await _context.Users.FirstOrDefaultAsync (u =>
			u.PhoneNumber == request.Phone && u.Password == request.Password.Hash (HashType.SHA256));

		// TODO: Verify captcha

		if (user == null)
		{
			throw new RpcException (new Status (StatusCode.NotFound, "Not Found"), metadata);
		}

		if (!user.Enabled)
		{
			throw new RpcException (new Status (StatusCode.PermissionDenied, "Permission Denied"), metadata);
		}

		return new GetUserReply
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
	                                                                ServerCallContext context)
	{
		Metadata metadata = new ();

		var user = await _context.Users.FirstOrDefaultAsync (u => u.Id == Guid.Parse (request.Id));

		if (user == null)
		{
			throw new RpcException (new Status (StatusCode.NotFound, "Not Found"), metadata);
		}

		user.Password = request.Password;

		await _context.SaveChangesAsync ();

		return new VoidOperationReply
		{
			Success = true
		};
	}

	public override async Task<VoidOperationReply> AddUser (AddUserRequest request, ServerCallContext context)
	{
		Metadata metadata = new ();

		var userExists =
			await _context.Users.AnyAsync (u => u.PhoneNumber == request.Phone);

		if (userExists)
		{
			throw new RpcException (new Status (StatusCode.AlreadyExists, "Already Exists"), metadata);
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

		_context.Add (user);
		await _context.SaveChangesAsync ();

		return new VoidOperationReply ();
	}
}