﻿using System.Text.Json;
using Microsoft.JSInterop;

namespace Bargeh.Main.Wapp.Client.Services;

public class LocalStorageService(IJSRuntime jsRuntime)
{
	public async Task<T?> GetItemAsync<T>(string key) where T : class
	{
		string? json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

		// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		return json is null
				   ?

				   // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				   null
				   : JsonSerializer.Deserialize<T>(json);
	}


	public async Task SetItemAsync<T>(string key, T value)
	{
		await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
	}

	public async Task RemoveItemAsync(string key)
	{
		await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	public static string? GetAccessToken(string? rawTokenString)
	{
		if(string.IsNullOrWhiteSpace(rawTokenString))
		{
			return null;
		}
		
		int atIndex = rawTokenString.IndexOf('@');
		if (atIndex != -1)
		{ 
			rawTokenString = rawTokenString[..atIndex];
			rawTokenString = rawTokenString.Replace("\"", "");
		}

		return rawTokenString;
	}
	
}