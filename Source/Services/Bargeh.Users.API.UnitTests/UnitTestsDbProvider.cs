using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Xunit.Abstractions;

namespace Bargeh.Users.API.UnitTests;

public static class UnitTestsDbProvider
{
	public static async Task PreparePostgresDb ()
	{
		try
		{
			await new HttpClient ().GetAsync ("http://localhost:5432");
		}
		catch (HttpRequestException exception)
		{
			if (exception.InnerException!.GetType () != typeof (HttpIOException))
			{
				ProcessStartInfo startInfo = new ()
				{
					FileName = "docker",
					Arguments = "run --name test-postgres -e POSTGRES_PASSWORD=5 -e POSTGRES_USER=postgres -e POSTGRES_DB=postgres -p 5432:5432 -d postgres",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				};

				Process process = new() { StartInfo = startInfo };
				process.Start ();

				await process.WaitForExitAsync ();
			}
		}
	}
}
