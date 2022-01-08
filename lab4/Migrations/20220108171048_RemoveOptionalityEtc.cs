using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class RemoveOptionalityEtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportEntries_Categories_ProjectCode_CategoryCode",
                table: "ReportEntries");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "ReportEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEntries_Categories_ProjectCode_CategoryCode",
                table: "ReportEntries",
                columns: new[] { "ProjectCode", "CategoryCode" },
                principalTable: "Categories",
                principalColumns: new[] { "ProjectCode", "Code" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportEntries_Categories_ProjectCode_CategoryCode",
                table: "ReportEntries");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "ReportEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEntries_Categories_ProjectCode_CategoryCode",
                table: "ReportEntries",
                columns: new[] { "ProjectCode", "CategoryCode" },
                principalTable: "Categories",
                principalColumns: new[] { "ProjectCode", "Code" });
        }
    }
}
