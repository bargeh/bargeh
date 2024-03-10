using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Topics.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     name: "Topics",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         ForumId = table.Column<Guid>(type: "uuid", nullable: false),
                                         Permalink = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                                         Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Topics", x => x.Id);
                                     });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "Topics");
    }
}