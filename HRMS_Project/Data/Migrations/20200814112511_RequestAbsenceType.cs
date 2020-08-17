using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class RequestAbsenceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbsenceTypeRef",
                table: "Request",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Request_AbsenceTypeRef",
                table: "Request",
                column: "AbsenceTypeRef");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request",
                column: "AbsenceTypeRef",
                principalTable: "AbsenceType",
                principalColumn: "IdAbsenceType",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_AbsenceTypeRef",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "AbsenceTypeRef",
                table: "Request");
        }
    }
}
