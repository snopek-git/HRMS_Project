using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class AddAbsence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsenceType",
                columns: table => new
                {
                    IdAbsenceType = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AbsenceTypeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AbsenceType_pk", x => x.IdAbsenceType);
                });

            migrationBuilder.CreateTable(
                name: "AvailableAbsence",
                columns: table => new
                {
                    IdAvailableAbsence = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableDays = table.Column<int>(nullable: false),
                    UsedAbsence = table.Column<int>(nullable: false, defaultValue: 0),
                    IdAbsenceType = table.Column<int>(nullable: false),
                    IdEmployee = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AvailableAbsence_pk", x => x.IdAvailableAbsence);
                    table.ForeignKey(
                        name: "AvailableAbsence_AbsenceType",
                        column: x => x.IdAbsenceType,
                        principalTable: "AbsenceType",
                        principalColumn: "IdAbsenceType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "AvailableAbsence_Emp",
                        column: x => x.IdEmployee,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableAbsence_IdAbsenceType",
                table: "AvailableAbsence",
                column: "IdAbsenceType");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableAbsence_IdEmployee",
                table: "AvailableAbsence",
                column: "IdEmployee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableAbsence");

            migrationBuilder.DropTable(
                name: "AbsenceType");
        }
    }
}
