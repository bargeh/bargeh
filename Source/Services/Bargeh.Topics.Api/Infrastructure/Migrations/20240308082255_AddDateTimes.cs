using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bargeh.Topics.Api.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDateTimes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
                                             name: "CreateDate",
                                             table: "Topics",
                                             type: "timestamp with time zone",
                                             nullable: false,
                                             defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
                                             name: "LastUpdateDate",
                                             table: "Topics",
                                             type: "timestamp with time zone",
                                             nullable: false,
                                             defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
                                    name: "CreateDate",
                                    table: "Topics");

        migrationBuilder.DropColumn(
                                    name: "LastUpdateDate",
                                    table: "Topics");
    }
}