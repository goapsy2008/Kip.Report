using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kip.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFieldsToReportQuery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                table: "report_queries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "report_json",
                table: "report_queries",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "started_at",
                table: "report_queries",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_completed",
                table: "report_queries");

            migrationBuilder.DropColumn(
                name: "report_json",
                table: "report_queries");

            migrationBuilder.DropColumn(
                name: "started_at",
                table: "report_queries");
        }
    }
}
