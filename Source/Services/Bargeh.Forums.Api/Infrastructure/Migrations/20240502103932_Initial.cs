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
                                         Avatar = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                                         Cover = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                                         Members = table.Column<long>(type: "bigint", nullable: false),
                                         Supporters = table.Column<long>(type: "bigint", nullable: false),
                                         OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Forums", x => x.Id);
                                     });

        migrationBuilder.CreateTable(
                                     name: "Topics",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         Forum = table.Column<Guid>(type: "uuid", nullable: false),
                                         Permalink = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                                         Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                                         LastUpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Topics", x => x.Id);
                                     });

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

        migrationBuilder.CreateTable(
                                     name: "Posts",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                                         ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                                         Attachment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
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

        migrationBuilder.CreateTable(
                                     name: "Reactions",
                                     columns: table => new
                                     {
                                         Id = table.Column<Guid>(type: "uuid", nullable: false),
                                         UserId = table.Column<Guid>(type: "uuid", nullable: false),
                                         PostId = table.Column<Guid>(type: "uuid", nullable: false),
                                         ReactionType = table.Column<int>(type: "integer", nullable: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_Reactions", x => x.Id);
                                         table.ForeignKey(
                                                          name: "FK_Reactions_Posts_PostId",
                                                          column: x => x.PostId,
                                                          principalTable: "Posts",
                                                          principalColumn: "Id",
                                                          onDelete: ReferentialAction.Cascade);
                                     });

        migrationBuilder.CreateIndex(
                                     name: "IX_ForumMemberships_ForumId",
                                     table: "ForumMemberships",
                                     column: "ForumId");

        migrationBuilder.CreateIndex(
                                     name: "IX_Posts_ParentId",
                                     table: "Posts",
                                     column: "ParentId");

        migrationBuilder.CreateIndex(
                                     name: "IX_Posts_TopicId",
                                     table: "Posts",
                                     column: "TopicId");

        migrationBuilder.CreateIndex(
                                     name: "IX_Reactions_PostId",
                                     table: "Reactions",
                                     column: "PostId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "ForumMemberships");

        migrationBuilder.DropTable(
                                   name: "Reactions");

        migrationBuilder.DropTable(
                                   name: "Forums");

        migrationBuilder.DropTable(
                                   name: "Posts");

        migrationBuilder.DropTable(
                                   name: "Topics");
    }
}