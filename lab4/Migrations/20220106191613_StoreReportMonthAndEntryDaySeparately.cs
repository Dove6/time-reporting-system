using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class StoreReportMonthAndEntryDaySeparately : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcceptedTime_Reports_ReportId",
                table: "AcceptedTime");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEntries_Reports_ReportId",
                table: "ReportEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_ReportEntries_ReportId",
                table: "ReportEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcceptedTime",
                table: "AcceptedTime");

            migrationBuilder.DropIndex(
                name: "IX_AcceptedTime_ProjectCode",
                table: "AcceptedTime");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "ReportEntries",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "ReportEntries",
                newName: "ReportMonth");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "AcceptedTime",
                newName: "OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "DayOfMonth",
                table: "ReportEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReportMonth",
                table: "AcceptedTime",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                columns: new[] { "OwnerId", "Month" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcceptedTime",
                table: "AcceptedTime",
                columns: new[] { "ProjectCode", "OwnerId", "ReportMonth" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reports_CK_Report_Month",
                table: "Reports",
                sql: "[Month] LIKE '____-__' AND strftime('%s', [Month] || '-01') IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportEntries_OwnerId_ReportMonth",
                table: "ReportEntries",
                columns: new[] { "OwnerId", "ReportMonth" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReportEntries_CK_ReportEntry_DayOfMonth",
                table: "ReportEntries",
                sql: "[DayOfMonth] LIKE '__' AND strftime('%s', [ReportMonth] || '-' || [DayOfMonth]) IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedTime_OwnerId_ReportMonth",
                table: "AcceptedTime",
                columns: new[] { "OwnerId", "ReportMonth" });

            migrationBuilder.AddForeignKey(
                name: "FK_AcceptedTime_Reports_OwnerId_ReportMonth",
                table: "AcceptedTime",
                columns: new[] { "OwnerId", "ReportMonth" },
                principalTable: "Reports",
                principalColumns: new[] { "OwnerId", "Month" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcceptedTime_Users_OwnerId",
                table: "AcceptedTime",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEntries_Reports_OwnerId_ReportMonth",
                table: "ReportEntries",
                columns: new[] { "OwnerId", "ReportMonth" },
                principalTable: "Reports",
                principalColumns: new[] { "OwnerId", "Month" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEntries_Users_OwnerId",
                table: "ReportEntries",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcceptedTime_Reports_OwnerId_ReportMonth",
                table: "AcceptedTime");

            migrationBuilder.DropForeignKey(
                name: "FK_AcceptedTime_Users_OwnerId",
                table: "AcceptedTime");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEntries_Reports_OwnerId_ReportMonth",
                table: "ReportEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEntries_Users_OwnerId",
                table: "ReportEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reports_CK_Report_Month",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_ReportEntries_OwnerId_ReportMonth",
                table: "ReportEntries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ReportEntries_CK_ReportEntry_DayOfMonth",
                table: "ReportEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcceptedTime",
                table: "AcceptedTime");

            migrationBuilder.DropIndex(
                name: "IX_AcceptedTime_OwnerId_ReportMonth",
                table: "AcceptedTime");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "ReportEntries");

            migrationBuilder.DropColumn(
                name: "ReportMonth",
                table: "AcceptedTime");

            migrationBuilder.RenameColumn(
                name: "ReportMonth",
                table: "ReportEntries",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "ReportEntries",
                newName: "ReportId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "AcceptedTime",
                newName: "ReportId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcceptedTime",
                table: "AcceptedTime",
                columns: new[] { "ReportId", "ProjectCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportEntries_ReportId",
                table: "ReportEntries",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedTime_ProjectCode",
                table: "AcceptedTime",
                column: "ProjectCode");

            migrationBuilder.AddForeignKey(
                name: "FK_AcceptedTime_Reports_ReportId",
                table: "AcceptedTime",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEntries_Reports_ReportId",
                table: "ReportEntries",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
