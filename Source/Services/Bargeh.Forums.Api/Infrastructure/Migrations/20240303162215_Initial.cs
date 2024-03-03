using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Forums.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     name: "Forums",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                                         Description = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                                         Permalink = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                                         Members = table.Column<long>(type: "bigint", nullable: false),
                                         Supporters = table.Column<long>(type: "bigint", nullable: false),
                                         OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Forums", x => x.Id);
                                     });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "Forums");
    }
}