using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BargehMigrations.Migrations
{
    /// <inheritdoc />
    public partial class UsersDbInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: false),
                    DisplayName = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: true),
                    Bio = table.Column<string>(type: "longtext", nullable: false),
                    ProfileImage = table.Column<string>(type: "longtext", nullable: false),
                    CoverImage = table.Column<string>(type: "longtext", nullable: false),
                    PremiumDaysLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    OnlineDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VerificationCode = table.Column<string>(type: "longtext", nullable: false),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanCreateForums = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
