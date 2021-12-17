using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class FixSqliteTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Users",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Reports",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "ReportEntries",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Projects",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Categories",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "AcceptedTime",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "randomblob(8)",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Users",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Reports",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "ReportEntries",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Projects",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Categories",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "AcceptedTime",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "randomblob(8)");
        }
    }
}
