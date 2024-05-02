using Bargeh.Forums.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Forums.Api.Infrastructure;

public static class ForumsDbInitializer
{
	public static async Task InitializeDbAsync(ForumsDbContext dbContext, ILogger logger)
	{
		TryConnect:

		try
		{
			await dbContext.Database.MigrateAsync();

			Forum forum = new()
			{
				Id = new("f84bdfa8-758c-4580-ac07-8891352541ea"),
				Name = "انجمن آزمایشی برگه",
				Description =
					"لورم ایپسوم یک متن آزمایشی و بی\u200cمعنی است که در صنعت چاپ و طراحی گرافیک استفاده می\u200cشود. این متن به طور معمول برای نمایش چاپی و ترتیب موقعیت حروف و کاراکترها در طراحی\u200cهای گرافیکی به کار می\u200cرود. هدف اصلی مقاله\u200cها و ارائه\u200cهای متنی این است که مخاطب را با محتوای متن آشنا سازد، نه آنکه نظر خاصی را درباره محتوا ارائه دهد",

				// ReSharper disable once StringLiteralTypo
				Permalink = "perma",
				OwnerId = new("9844fd47-3236-46cb-898d-607b5c5563c1")
			};

			await dbContext.AddAsync(forum);
			await dbContext.SaveChangesAsync();

			#region Seed Data

			Topic topic = new()
			{
				Id = new("29cb3ae1-3c02-4d49-a0d8-5a65dcfe958f"),
				Forum = new("f84bdfa8-758c-4580-ac07-8891352541ea"),
				Title = "تیتر تاپیک"
			};

			await dbContext.AddAsync(topic);
			await dbContext.SaveChangesAsync();

			Post headPost = new()
			{
				Topic = topic,
				Author = new("9844fd47-3236-46cb-898d-607b5c5563c1"),
				Body = "هد پست که می‌گن اینه ها!"
			};

			await dbContext.AddAsync(headPost);
			await dbContext.SaveChangesAsync();

			for(int i = 0; i < 100; i++)
			{
				Post topLevelPost = new()
				{
					Topic = topic,
					Author = new("9844fd47-3236-46cb-898d-607b5c5563c1"),
					Body = $"این پست تاپ چین هست با اندیس {i}",
					Parent = headPost
				};

				await dbContext.AddAsync(topLevelPost);
				await dbContext.SaveChangesAsync();

				// Create child post chain
				Random random = new();
				int maxLevels = random.Next(1, 11); // Random number of levels between 1 and 10
				Guid? currentPostId = topLevelPost.Id;

				for(int j = 0; j < maxLevels; j++)
				{
					Post childPost = new()
					{
						Topic = topic,
						Author = new("9844fd47-3236-46cb-898d-607b5c5563c1"),
						Body = $"این پست فرزند سطح {j + 1} است.",
						Parent = dbContext.Posts.FirstOrDefault(p => p.Id == currentPostId)
					};

					await dbContext.AddAsync(childPost);
					await dbContext.SaveChangesAsync();
					currentPostId = childPost.Id;
				}
			}

			#endregion

			logger.LogDebug("Forums database initialization completed successfully");
		}
		catch
		{
			goto TryConnect;
		}
	}
}