using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Topics.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPermalink : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
                                           name: "Permalink",
                                           table: "Topics",
                                           type: "character varying(64)",
                                           maxLength: 64,
                                           nullable: false,
                                           defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
                                    name: "Permalink",
                                    table: "Topics");
    }
}