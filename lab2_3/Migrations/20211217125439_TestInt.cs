using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class TestInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestInt",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestInt",
                table: "Users");
        }
    }
}
