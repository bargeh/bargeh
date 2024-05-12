using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Bargeh.Admin.Api.Services;

public class AdminService(UsersProtoClient usersService)
	: AdminProto.AdminProtoBase
{
	public override async Task<Empty> AddUser(AddUserRequest request, ServerCallContext context)
	{
	}
}