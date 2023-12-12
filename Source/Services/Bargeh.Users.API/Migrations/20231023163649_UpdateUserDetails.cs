using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BargehMigrations.Migrations
{
	/// <inheritdoc />
	public partial class UpdateUserDetails : Migration
	{
		/// <inheritdoc />
		protected override void Up (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string> (
				name: "Email",
				table: "Users",
				type: "longtext",
				nullable: true,
				oldClrType: typeof (string),
				oldType: "longtext");

			migrationBuilder.AddColumn<DateTime> (
				name: "BirthDate",
				table: "Users",
				type: "datetime(6)",
				nullable: true);

			migrationBuilder.AddColumn<bool> (
				name: "IsMale",
				table: "Users",
				type: "tinyint(1)",
				nullable: true);

			migrationBuilder.AddColumn<string> (
				name: "PhoneNumber",
				table: "Users",
				type: "longtext",
				nullable: false);

			migrationBuilder.AddColumn<int> (
				name: "Province",
				table: "Users",
				type: "int",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn (
				name: "BirthDate",
				table: "Users");

			migrationBuilder.DropColumn (
				name: "IsMale",
				table: "Users");

			migrationBuilder.DropColumn (
				name: "PhoneNumber",
				table: "Users");

			migrationBuilder.DropColumn (
				name: "Province",
				table: "Users");

			migrationBuilder.AlterColumn<string> (
				name: "Email",
				table: "Users",
				type: "longtext",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof (string),
				oldType: "longtext",
				oldNullable: true);
		}
	}
}
