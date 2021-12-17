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
                table: "Users",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Reports",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "ReportEntries",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Projects",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Categories",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "AcceptedTime",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ReportEntries");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AcceptedTime");
        }
    }
}
