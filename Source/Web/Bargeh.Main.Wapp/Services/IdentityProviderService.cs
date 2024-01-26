namespace Bargeh.Main.Wapp.Services;

public class IdentityProviderService (LocalStorageService localStorageService)
{
	public async Task GetUserIdentity ()
	{
		await localStorageService.SetItemAsync("what a?", "huh?");
	}
}