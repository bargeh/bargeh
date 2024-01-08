using Users.Api;

namespace Bargeh.Main.Wapp.Infrastructure.GrpcProviders;

public class UsersApiGrpcProvider (UsersProto.UsersProtoClient client)
{
	public string GetUserById ()
	{
		return client.GetUserById (new ()
		{
			Id = "9844fd47-3236-46cb-898d-607b5c5563c1"
		}).Username;
	}
}