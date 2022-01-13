using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class RemoveTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_AcceptedTime_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_AcceptedTime_Timestamp_INS");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_Project_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_Project_Timestamp_INS");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_ReportEntry_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_ReportEntry_Timestamp_INS");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_AcceptedTime_Timestamp_UPD
                AFTER UPDATE ON AcceptedTime
                BEGIN
                    UPDATE AcceptedTime
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_AcceptedTime_Timestamp_INS
                AFTER INSERT ON AcceptedTime
                BEGIN
                    UPDATE AcceptedTime
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_Projects_Timestamp_UPD
                AFTER UPDATE ON Projects
                BEGIN
                    UPDATE Projects
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_Projects_Timestamp_INS
                AFTER INSERT ON Projects
                BEGIN
                    UPDATE Projects
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_ReportEntries_Timestamp_UPD
                AFTER UPDATE ON ReportEntries
                BEGIN
                    UPDATE ReportEntries
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.Sql(@"CREATE TRIGGER IF NOT EXISTS TRIG_ReportEntries_Timestamp_INS
                AFTER INSERT ON ReportEntries
                BEGIN
                    UPDATE ReportEntries
                    SET Timestamp = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
        }
    }
}
