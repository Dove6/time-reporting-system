using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trs.Migrations
{
    public partial class AddSqliteSpecificTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // source: https://stackoverflow.com/questions/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexce
            // and: https://khalidabuhakmeh.com/raw-sql-queries-with-ef-core-5

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_AcceptedTime_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_AcceptedTime_Timestamp_INS");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_Project_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_Project_Timestamp_INS");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_ReportEntry_Timestamp_UPD");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TRIG_ReportEntry_Timestamp_INS");
        }
    }
}
