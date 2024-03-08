using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Topics.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Init : Migration
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
                                         AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                                         Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                                         Body = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                                         Likes = table.Column<long>(type: "bigint", nullable: false),
                                         Loves = table.Column<long>(type: "bigint", nullable: false),
                                         Funnies = table.Column<long>(type: "bigint", nullable: false),
                                         Insights = table.Column<long>(type: "bigint", nullable: false),
                                         Dislikes = table.Column<long>(type: "bigint", nullable: false)
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