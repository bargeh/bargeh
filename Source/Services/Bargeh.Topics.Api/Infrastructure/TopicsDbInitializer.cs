using System.Text;
using Bargeh.Topics.Api.Infrastructure.Models;
using Bargeh.Topics.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Topics.Api.Infrastructure;

public static class TopicsDbInitializer
{
	public static async Task InitializeDbAsync(TopicsDbContext dbContext, ILogger logger)
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

		#region Seed Data

		Topic topic = new()
		{
			Id = new Guid("29cb3ae1-3c02-4d49-a0d8-5a65dcfe958f"),
			Forum = new Guid("f84bdfa8-758c-4580-ac07-8891352541ea"),
			Title = "تیتر تاپیک"
		};

		await dbContext.AddAsync(topic);
		await dbContext.SaveChangesAsync();

		Post headPost = new()
		{
			Topic = topic,
			Author = new Guid("9844fd47-3236-46cb-898d-607b5c5563c1"),
			Body = "هد پست که می‌گن اینه ها!"
		};

		await dbContext.AddAsync(headPost);
		await dbContext.SaveChangesAsync();

		List<Post> topLevelPosts = new List<Post>();

		for(int i = 0; i < 100; i++)
		{
			Post topLevelPost = new()
			{
				Topic = topic,
				Author = new Guid("9844fd47-3236-46cb-898d-607b5c5563c1"),
				Body = $"این پست تاپ چین هست با اندیس {i}",
				Parent = headPost
			};

			topLevelPosts.Add(topLevelPost);
			await dbContext.AddAsync(topLevelPost);
			await dbContext.SaveChangesAsync();

			// Create child post chain
			Random random = new Random();
			int maxLevels = random.Next(1, 11); // Random number of levels between 1 and 10
			Guid? currentPostId = topLevelPost.Id;

			for(int j = 0; j < maxLevels; j++)
			{
				Post childPost = new Post
				{
					Topic = topic,
					Author = new Guid("9844fd47-3236-46cb-898d-607b5c5563c1"),
					Body = $"این پست فرزند سطح {j + 1} است.",
					Parent = dbContext.Posts.FirstOrDefault(p => p.Id == currentPostId)
				};

				await dbContext.AddAsync(childPost);
				await dbContext.SaveChangesAsync();
				currentPostId = childPost.Id;
			}
		}

		#endregion

		logger.LogDebug("Topics database initialization completed successfully");
	}
}