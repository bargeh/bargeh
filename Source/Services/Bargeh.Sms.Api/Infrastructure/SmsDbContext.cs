using Bargeh.Sms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Sms.Api.Infrastructure;

public class SmsDbContext (DbContextOptions<SmsDbContext> options) : DbContext (options)
{
	public DbSet<SmsVerification> SmsVerifications { get; set; }
}