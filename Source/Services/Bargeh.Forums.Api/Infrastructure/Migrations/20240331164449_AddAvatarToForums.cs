using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Forums.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAvatarToForums : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
                                           name: "Avatar",
                                           table: "Forums",
                                           type: "character varying(128)",
                                           maxLength: 128,
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<string>(
                                           name: "Cover",
                                           table: "Forums",
                                           type: "character varying(128)",
                                           maxLength: 128,
                                           nullable: false,
                                           defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
                                    name: "Avatar",
                                    table: "Forums");

        migrationBuilder.DropColumn(
                                    name: "Cover",
                                    table: "Forums");
    }
}