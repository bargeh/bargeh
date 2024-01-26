using Microsoft.JSInterop;
using System.Text.Json;

namespace Bargeh.Main.Wapp.Client.Services;

public class LocalStorageService (IJSRuntime jsRuntime)
{
	public async Task<T> GetItemAsync<T> (string key)
	{
		string json = await jsRuntime.InvokeAsync<string> ("localStorage.getItem", key);

		return JsonSerializer.Deserialize<T> (json)!;
	}

	public async Task SetItemAsync<T> (string key, T value)
	{
		await jsRuntime.InvokeVoidAsync ("localStorage.setItem", key, JsonSerializer.Serialize (value));
	}

	public async Task RemoveItemAsync (string key)
	{
		await jsRuntime.InvokeVoidAsync ("localStorage.removeItem", key);
	}
}
