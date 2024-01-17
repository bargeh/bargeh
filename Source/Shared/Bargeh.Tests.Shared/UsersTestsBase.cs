using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Models;
using Bargeh.Users.Api.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Bargeh.Tests.Shared;

public abstract class UsersTestsBase : IAsyncLifetime
{
    protected internal const string VALID_USER_ID = "9844fd47-3236-46cb-898d-607b5c5560c1";
    protected internal readonly TestsDbProvider DbProvider = new ();
    protected internal UsersContext Context = null!;
    protected internal string ConnectionString = null!;
    protected internal readonly ServerCallContext CallContext = TestServerCallContext.Create (
        "testMethod",
        null,
        DateTime.UtcNow,
        [],
        CancellationToken.None,
        "127.0.0.1",
        null,
        null,
        _ => Task.CompletedTask,
        () => new (),
        _ => { });

    public virtual async Task InitializeAsync ()
    {
        ConnectionString = await DbProvider.PreparePostgresDb ();
        DbContextOptionsBuilder<UsersContext> optionsBuilder = new ();
        optionsBuilder.UseNpgsql (ConnectionString);
        Context = new (optionsBuilder.Options);
        await UsersDbInitializer.InitializeDbAsync (Context, new Logger<UsersTestsBase> (new NullLoggerFactory ()));

        if (await Context.Users.AnyAsync ())
        {
            return;
        }

        User user = new ()
        {
            Id = new (VALID_USER_ID),
            Username = "test",
            DisplayName = "test display name",
            Email = "test@gmail.bargeh",
            VerificationCode = "0",
            Password = "5".Hash (HashType.SHA256),
            PhoneNumber = "09123456789"
        };

        Context.Add (user);
        await Context.SaveChangesAsync ();
    }

    public Task DisposeAsync ()
    {
        return Task.CompletedTask;
    }
}