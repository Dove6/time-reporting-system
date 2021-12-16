using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class CreateInitialStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Budget = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Projects_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Month = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Frozen = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Projects_ProjectCode",
                        column: x => x.ProjectCode,
                        principalTable: "Projects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedTime",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedTime", x => new { x.ReportId, x.ProjectCode });
                    table.ForeignKey(
                        name: "FK_AcceptedTime_Projects_ProjectCode",
                        column: x => x.ProjectCode,
                        principalTable: "Projects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcceptedTime_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Time = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReportId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportEntries_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportEntries_Projects_ProjectCode",
                        column: x => x.ProjectCode,
                        principalTable: "Projects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportEntries_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedTime_ProjectCode",
                table: "AcceptedTime",
                column: "ProjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProjectCode_Name",
                table: "Categories",
                columns: new[] { "ProjectCode", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ManagerId",
                table: "Projects",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportEntries_CategoryId",
                table: "ReportEntries",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportEntries_ProjectCode",
                table: "ReportEntries",
                column: "ProjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_ReportEntries_ReportId",
                table: "ReportEntries",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_OwnerId_Month",
                table: "Reports",
                columns: new[] { "OwnerId", "Month" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedTime");

            migrationBuilder.DropTable(
                name: "ReportEntries");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
