using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Users.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     name: "RefreshTokens",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                                         UserId = table.Column<Guid>(type: "uuid", nullable: false),
                                         ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                                     });

        migrationBuilder.CreateTable(
                                     name: "SmsVerifications",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Code = table.Column<int>(type: "integer", nullable: false),
                                         Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                                         ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_SmsVerifications", x => x.Id);
                                     });

        migrationBuilder.CreateTable(
                                     name: "Users",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Username = table.Column<string>(type: "text", nullable: false),
                                         DisplayName = table.Column<string>(type: "text", nullable: false),
                                         PhoneNumber = table.Column<string>(type: "text", nullable: false),
                                         Password = table.Column<string>(type: "text", nullable: true),
                                         Bio = table.Column<string>(type: "text", nullable: false),
                                         Avatar = table.Column<string>(type: "text", nullable: false),
                                         Cover = table.Column<string>(type: "text", nullable: false),
                                         Followers = table.Column<long>(type: "bigint", nullable: false),
                                         PremiumDaysLeft = table.Column<int>(type: "integer", nullable: false),
                                         OnlineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                         RegisterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                         Enabled = table.Column<bool>(type: "boolean", nullable: false),
                                         CanCreateForums = table.Column<bool>(type: "boolean", nullable: false),
                                         Email = table.Column<string>(type: "text", nullable: true)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Users", x => x.Id);
                                     });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "RefreshTokens");

        migrationBuilder.DropTable(
                                   name: "SmsVerifications");

        migrationBuilder.DropTable(
                                   name: "Users");
    }
}