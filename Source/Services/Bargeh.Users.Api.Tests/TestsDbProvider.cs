using System.Diagnostics;

namespace Bargeh.Users.Api.Tests;

public class TestsDbProvider
{
	private readonly string _dbName = "db" + Random.Shared.Next(1024, 8192);

	//	public string PreparePostgresDb ()
	//	{
	//		try
	//		{
	//			HttpClient httpClient = new()
	//			{
	//				Timeout = TimeSpan.FromSeconds(1)
	//			};

	//			httpClient.GetAsync ("http://localhost:5432").Wait ();
	//		}
	//		catch (Exception exception)
	//		{
	//			if (exception.InnerException!.GetType () != typeof (HttpIOException))
	//			{
	//				ProcessStartInfo startInfo = new ()
	//				{
	//					FileName = "docker",
	//					Arguments = $"run --name test-postgres -e POSTGRES_PASSWORD=5 -e POSTGRES_USER=postgres -e POSTGRES_DB=postgres -p 5432:5432 -d postgres",
	//					RedirectStandardOutput = true,
	//					UseShellExecute = false,
	//					CreateNoWindow = true,
	//				};

	//				Process process = new () { StartInfo = startInfo };
	//				process.Start ();

	//				process.WaitForExit ();
	//			}
	//		}

	//		return $"Host=localhost;Port=5432;Username=postgres;Password=5;Database={_dbName}";
	//	}
	//}

	public async Task<string> PreparePostgresDb ()
	{
		try
		{
			HttpClient httpClient = new ()
			{
				Timeout = TimeSpan.FromSeconds (1)
			};

			await httpClient.GetAsync ("http://localhost:5432");
		}
		catch (Exception exception)
		{
			if (exception.InnerException!.GetType () != typeof (HttpIOException))
			{
				ProcessStartInfo startInfo = new ()
				{
					FileName = "docker",
					Arguments = "run --name test-postgres -e POSTGRES_PASSWORD=5 -e POSTGRES_USER=postgres -e POSTGRES_DB=postgres -p 5432:5432 -d postgres",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				Process process = new () { StartInfo = startInfo };
				process.Start();

				await process.WaitForExitAsync ();
			}
		}

		return $"Host=localhost;Port=5432;Username=postgres;Password=5;Database={_dbName}";
	}
}