using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Aspire.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder (args);

builder.AddServiceDefaults ();

WebApplication app = builder.Build ();

app.MapDefaultEndpoints ();

app.MapPost ("/Login", (HttpContext context,
										 IConfiguration configuration,
										 [FromHeader] string phone,
										 [FromHeader] string password,
										 [FromHeader] string validationToken) =>
{
	string? value = configuration.GetValue<string>("services:usersapi:1");

	return value;
	//string token = GenerateJwt(phone);

	//context.Response.Cookies.Append ("jwt", token, new ()
	//{
	//	Expires = DateTime.UtcNow.AddSeconds (15),
	//	IsEssential = true,
	//	SameSite = SameSiteMode.Strict,
	//	HttpOnly = true
	//});
});

app.Run ();
string GenerateJwt (string username)
{
	RsaSecurityKey securityKey = new (new X509Certificate2 ("C:/Users/Matin/Desktop/private_key.pfx", "bargeh.dev").GetRSAPrivateKey ());

	SigningCredentials credentials = new (securityKey, SecurityAlgorithms.RsaSha256);

	List<Claim> claims =
	[
		new (ClaimsIdentity.DefaultNameClaimType, username)
		//new (JwtRegisteredClaimNames.Email, "matinmn87@gmail.com")
	];

	JwtSecurityToken jwtToken = new (
		issuer: "https://bargeh.net",
		audience: "https://bargeh.net",
		claims,
		expires: DateTime.UtcNow.AddMinutes (5),
		signingCredentials: credentials);

	string token = new JwtSecurityTokenHandler ().WriteToken (jwtToken);

	//Response.Cookies.Append ("jwt", token, new ()
	//{
	//	Expires = DateTime.UtcNow.AddSeconds (15),
	//	IsEssential = true,
	//	SameSite = SameSiteMode.Strict,
	//	HttpOnly = true
	//});

	return token;
}