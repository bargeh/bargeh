namespace Bargeh.Main.Wapp.Client.Services;

public class IdentityProviderService (LocalStorageService localStorageService)
{
	public async Task GetUserIdentity ()
	{
		await localStorageService.SetItemAsync ("what a?", "huh?");
	}
}