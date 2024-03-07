using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Forums.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddForumMemberships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     name: "ForumMemberships",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         UserId = table.Column<Guid>(type: "uuid", nullable: false),
                                         ForumId = table.Column<Guid>(type: "uuid", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_ForumMemberships", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_ForumMemberships_Forums_ForumId",
                                                          column: x => x.ForumId,
                                                          principalTable: "Forums",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Cascade);
                                     });

        migrationBuilder.CreateIndex(
                                     name: "IX_ForumMemberships_ForumId",
                                     table: "ForumMemberships",
                                     column: "ForumId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "ForumMemberships");
    }
}