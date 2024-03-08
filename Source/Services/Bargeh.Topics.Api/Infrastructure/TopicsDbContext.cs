using Bargeh.Topics.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Bargeh.Topics.Api.Infrastructure;

public class TopicsDbContext(DbContextOptions<TopicsDbContext> options) : DbContext(options)
{
	#region Database Objects

	public DbSet<Topic> Topics { get; set; }
	
	#endregion
}