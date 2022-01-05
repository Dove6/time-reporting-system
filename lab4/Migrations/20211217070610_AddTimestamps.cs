using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class AddTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "ReportEntries",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Projects",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "AcceptedTime",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ReportEntries");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AcceptedTime");
        }
    }
}
