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
                                         Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Topics", x => x.Id);
                                     });

        migrationBuilder.CreateTable(
                                     name: "Posts",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                                         ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                                         Attachment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                                         Media = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                                         Author = table.Column<Guid>(type: "uuid", nullable: false),
                                         LastUpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                         Body = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                                         Likes = table.Column<long>(type: "bigint", nullable: false),
                                         Loves = table.Column<long>(type: "bigint", nullable: false),
                                         Funnies = table.Column<long>(type: "bigint", nullable: false),
                                         Insights = table.Column<long>(type: "bigint", nullable: false),
                                         Dislikes = table.Column<long>(type: "bigint", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Posts", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_Posts_Posts_ParentId",
                                                          column: x => x.ParentId,
                                                          principalTable: "Posts",
                                                          principalColumn: "Id");
                                         table.ForeignKey(
                                                          name: "FK_Posts_Topics_TopicId",
                                                          column: x => x.TopicId,
                                                          principalTable: "Topics",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Cascade);
                                     });

        migrationBuilder.CreateIndex(
                                     name: "IX_Posts_ParentId",
                                     table: "Posts",
                                     column: "ParentId");

        migrationBuilder.CreateIndex(
                                     name: "IX_Posts_TopicId",
                                     table: "Posts",
                                     column: "TopicId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "Posts");

        migrationBuilder.DropTable(
                                   name: "Topics");
    }
}