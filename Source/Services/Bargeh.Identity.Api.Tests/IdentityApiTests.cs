using Bargeh.Identity.Api.Services;
using Bargeh.Tests.Shared;

namespace Bargeh.Identity.Api.Tests;

public class IdentityApiTests : UsersTestsBase
{
	private readonly TimeProvider _timeProvider = null!;
	private IdentityService _identityService = null!;

	//public override async Task InitializeAsync ()
	//{
	//    await base.InitializeAsync ();

	//    UsersProtoClient usersClient = Mock.Of<UsersProtoClient> ();

	//    DbContextOptionsBuilder<IdentityDbContext> optionsBuilder = new ();
	//    optionsBuilder.UseNpgsql (ConnectionString);

	//    IdentityDbContext identityDbContext = new (optionsBuilder.Options);
	//    await IdentityDbInitializer.InitializeDbAsync (identityDbContext, new NullLogger<IdentityDbContext> ());

	//    _identityService = new (usersClient, identityDbContext, _timeProvider);
	//}

	//[Fact]
	//public async Task Login_ThrowsIfUserIsInvalid ()
	//{
	//    // Arrange


	//    // Act


	//    // Assert

	//}
}