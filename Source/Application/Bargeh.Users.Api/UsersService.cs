using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Infrastructure.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MatinDevs.PersianPhoneNumbers;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Bargeh.Users.Api.Services;

public class UsersService(UsersDbContext dbContext, ILogger<UsersService> logger, IMediator mediator) : UsersProto.UsersProtoBase
{
	private readonly HashSet<char> _allowedCharsForUsername =
	[
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
		'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'_', '.'
	];

	public override async Task<ProtoUser> GetUserByUsername(GetUserByUsernameRequest request,
															ServerCallContext callContext)
	{
		#pragma warning disable CA1862
		User? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username);
		#pragma warning restore CA1862

		if(user == null)
		{
			throw new RpcException(new(StatusCode.NotFound, "Not Found"));
		}

		return new()
		{
			Id = user.Id.ToString(),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			Cover = user.Cover,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			Avatar = user.Avatar,
			Followers = user.Followers
		};
	}

	public override async Task<ProtoUser> GetUserByPhone(GetUserByPhoneRequest request,
														 ServerCallContext callContext)
	{
		User? user = await dbContext.GetUserByPhoneNumberAsync(request.Phone);

		if(user == null)
		{
			throw new RpcException(new(StatusCode.NotFound, "Not Found"));
		}

		return new()
		{
			Id = user.Id.ToString(),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			Cover = user.Cover,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			Avatar = user.Avatar,
			Followers = user.Followers
		};
	}

	public override async Task<ProtoUser> GetUserById(GetUserByIdRequest request,
													  ServerCallContext callContext)
	{
		User? user = await dbContext.GetUserByIdAsync(request.Id);

		if(user == null)
		{
			throw new RpcException(new(StatusCode.NotFound, "Not Found"));
		}

		return new()
		{
			Id = user.Id.ToString(),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			Cover = user.Cover,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			Avatar = user.Avatar,
			Followers = user.Followers
		};
	}

	public override async Task<ProtoUser> GetUserByPhoneAndPassword(GetUserByPhoneAndPasswordRequest request,
																	ServerCallContext callContext)
	{
		User? user = await dbContext.GetUserByPhoneAndPasswordAsync(request.Phone, request.Password);

		// PRODUCTION: Verify captcha

		if(user == null)
		{
			throw new RpcException(new(StatusCode.NotFound,
									   "The user with this phone number and password was not found"));
		}

		if(!user.Enabled)
		{
			throw new RpcException(new(StatusCode.FailedPrecondition, "The user is disabled"));
		}

		return new()
		{
			Id = user.Id.ToString(),
			Username = user.Username,
			Bio = user.Bio,
			CanCreateForums = user.CanCreateForums,
			Cover = user.Cover,
			DisplayName = user.DisplayName,
			Email = user.Email ?? string.Empty,
			Enabled = user.Enabled,
			PremiumDaysLeft = user.PremiumDaysLeft,
			Avatar = user.Avatar,
			Followers = user.Followers
		};
	}

	public override async Task<Empty> SetUserPassword(SetUserPasswordRequest request,
													  ServerCallContext callContext)
	{
		// PRODUCTION: Rebuild this method

		User user = await dbContext.GetUserByPhoneNumberAsync(request.Phone) ??
					throw new RpcException(new(StatusCode.NotFound, "Not Found"));

		user.Password = request.Password.Hash(HashType.SHA256);

		await dbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<Empty> AddUser(AddUserRequest request, ServerCallContext callContext)
	{
		// PRODUCTION: Remove captcha

		if(!request.AcceptedTos)
		{
			throw new RpcException(new(StatusCode.FailedPrecondition,
									   "You have to accept Bargeh's Terms of Service in order to create a new account"));
		}

		if(string.IsNullOrWhiteSpace(request.DisplayName))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Display Name\" is not valid"));
		}


		if(string.IsNullOrWhiteSpace(request.Username) || request.Username.Length <= 4 ||
		   !request.Username.ToLower().All(c => _allowedCharsForUsername.Contains(c)))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Username\" is not valid"));
		}

		if(!request.Phone.ConvertToEnglish().IsPersianPhoneValid())
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Phone Number\" is not valid"));
		}

		bool userExists =
			await dbContext.UserExistsByPhoneAsync(request.Phone.ConvertToEnglish());

		if(userExists)
		{
			throw new RpcException(new(StatusCode.AlreadyExists, "Already Exists"));
		}

		int discriminator = Random.Shared.Next(1000, 9999);

		string username = request.Username + discriminator;
		string displayName = request.DisplayName;

		User user = new()
		{
			PhoneNumber = request.Phone.ConvertToEnglish(),
			DisplayName = displayName,
			Username = username
		};

		await dbContext.AddAsync(user, callContext.CancellationToken);
		await dbContext.SaveChangesAsync(callContext.CancellationToken);

		return new();
	}

	public override async Task<Empty> DisableUser(DisableUserRequest request,
												  ServerCallContext callContext)
	{
		ProtoUser user = await GetUserById(new()
										   {
											   Id = request.Id
										   },
										   callContext);

		if(user is null)
		{
			throw new RpcException(new(StatusCode.NotFound, "User not found"));
		}

		User dbUser = (await dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == request.Id))!;

		dbUser.Enabled = false;

		await dbContext.SaveChangesAsync();

		return new();
	}
}
