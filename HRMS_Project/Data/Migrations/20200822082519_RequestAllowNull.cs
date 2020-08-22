using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class RequestAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "AbsenceTypeRef",
                table: "Request",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request",
                column: "AbsenceTypeRef",
                principalTable: "AbsenceType",
                principalColumn: "IdAbsenceType",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "AbsenceTypeRef",
                table: "Request",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AbsenceType_AbsenceTypeRef",
                table: "Request",
                column: "AbsenceTypeRef",
                principalTable: "AbsenceType",
                principalColumn: "IdAbsenceType",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
