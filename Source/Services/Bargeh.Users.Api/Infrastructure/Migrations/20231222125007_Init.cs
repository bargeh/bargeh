using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Users.Api.Infrastructure.Migrations;

	/// <inheritdoc />
	public partial class Init : Migration
	{
		/// <inheritdoc />
		protected override void Up (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable (
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid> (type: "uuid", nullable: false),
					Username = table.Column<string> (type: "text", nullable: false),
					DisplayName = table.Column<string> (type: "text", nullable: false),
					PhoneNumber = table.Column<string> (type: "text", nullable: false),
					Password = table.Column<string> (type: "text", nullable: true),
					Bio = table.Column<string> (type: "text", nullable: false),
					ProfileImage = table.Column<string> (type: "text", nullable: false),
					CoverImage = table.Column<string> (type: "text", nullable: false),
					PremiumDaysLeft = table.Column<int> (type: "integer", nullable: false),
					OnlineDate = table.Column<DateTime> (type: "timestamp with time zone", nullable: false),
					RegisterDate = table.Column<DateTime> (type: "timestamp with time zone", nullable: false),
					Enabled = table.Column<bool> (type: "boolean", nullable: false),
					CanCreateForums = table.Column<bool> (type: "boolean", nullable: false),
					Email = table.Column<string> (type: "text", nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey ("PK_Users", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable (
				name: "Users");
		}
	}
