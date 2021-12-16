using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class RenameCategoryNameToCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ProjectCode_Name",
                table: "Categories",
                newName: "IX_Categories_ProjectCode_Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ProjectCode_Code",
                table: "Categories",
                newName: "IX_Categories_ProjectCode_Name");
        }
    }
}
