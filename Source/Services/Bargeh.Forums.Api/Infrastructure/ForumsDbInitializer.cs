using Bargeh.Forums.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Forums.Api.Infrastructure;

public static class ForumsDbInitializer
{
	public static async Task InitializeDbAsync(ForumsDbContext dbContext, ILogger logger)
	{
		byte retires = 20;

		TryConnect:

		if(!await dbContext.Database.CanConnectAsync() && retires >= 1)
		{
			await Task.Delay(1000);
			logger.LogInformation("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		try
		{
			await dbContext.Database.MigrateAsync();
		}
		catch
		{
			// ignored
		}

		Forum forum = new()
		{
			Id = new("f84bdfa8-758c-4580-ac07-8891352541ea"),
			Name = "انجمن آزمایشی برگه",
			Description = "لورم ایپسوم یک متن آزمایشی و بی\u200cمعنی است که در صنعت چاپ و طراحی گرافیک استفاده می\u200cشود. این متن به طور معمول برای نمایش چاپی و ترتیب موقعیت حروف و کاراکترها در طراحی\u200cهای گرافیکی به کار می\u200cرود. هدف اصلی مقاله\u200cها و ارائه\u200cهای متنی این است که مخاطب را با محتوای متن آشنا سازد، نه آنکه نظر خاصی را درباره محتوا ارائه دهد",
			// ReSharper disable once StringLiteralTypo
			Permalink = "perma",
			OwnerId = new("9844fd47-3236-46cb-898d-607b5c5563c1")
		};

		await dbContext.AddAsync(forum);
		await dbContext.SaveChangesAsync();

		logger.LogDebug("Forums database initialization completed successfully");
	}
}