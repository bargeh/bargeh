using Bargeh.Identity.Api.Infrastructure;
using Bargeh.Identity.Api.Services;
using Bargeh.Tests.Shared;
using Grpc.Core;
using Moq;
using Users.Api;

namespace Bargeh.Identity.Api.Tests;

public class IdentityApiTests : UsersTestsBase
{
    private TimeProvider _timeProvider = null!;
}