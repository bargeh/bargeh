using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Identity.Api.Migrations
{
	/// <inheritdoc />
	public partial class Init : Migration
	{
		/// <inheritdoc />
		protected override void Up (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable (
				name: "RefreshTokens",
				columns: table => new
				{
					Id = table.Column<Guid> (type: "uuid", nullable: false),
					Token = table.Column<string> (type: "text", nullable: false),
					UserId = table.Column<Guid> (type: "uuid", nullable: false),
					ExpireDate = table.Column<DateTime> (type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey ("PK_RefreshTokens", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable (
				name: "RefreshTokens");
		}
	}
}
