using System.Diagnostics;

namespace Bargeh.Users.API.UnitTests;

public class UnitTestsDbProvider
{
	private readonly int _databaseId = Random.Shared.Next (1025, 8191);
	private readonly string _containerName;

	public UnitTestsDbProvider ()
	{
		_containerName = $"test-postgres-{_databaseId}";
	}

	public async Task<string> PreparePostgresDbAsync ()
	{
		ProcessStartInfo startInfo = new ()
		{
			FileName = "docker",
			Arguments =
				$"run --name {_containerName} -e POSTGRES_PASSWORD=5 -e POSTGRES_USER=postgres -e POSTGRES_DB=postgres -p {_databaseId}:5432 -d postgres",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
		};

		Process process = new () { StartInfo = startInfo };
		process.Start ();

		await process.WaitForExitAsync ();

		return $"Host=localhost;Port={_databaseId};Username=postgres;Password=5;Database=postgres";
	}

	public async Task DisposePostgresDbAsync ()
	{
		ProcessStartInfo startInfo = new ()
		{
			FileName = "docker",
			Arguments = $"rm -f {_containerName}",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
		};

		Process process = new () { StartInfo = startInfo };
		process.Start ();

		await process.WaitForExitAsync ();
	}
}
