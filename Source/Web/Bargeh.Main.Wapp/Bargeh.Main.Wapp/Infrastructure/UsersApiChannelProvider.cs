using Grpc.Net.Client;
using Users.API;

namespace Bargeh.Main.Wapp.Client.Infrastructure;

public class UsersApiGrpcClientProvider
{
    private readonly UsersProto.UsersProtoClient _client;

    public UsersApiGrpcClientProvider (string address)
    {
        var channel = GrpcChannel.ForAddress (address);
        _client = new (channel);
    }

    public UsersProto.UsersProtoClient GetUsersApiClient ()
    {
        return _client;
    }
}