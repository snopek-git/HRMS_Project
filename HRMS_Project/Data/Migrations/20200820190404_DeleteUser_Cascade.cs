using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class DeleteUser_Cascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "AvailableAbsence_Emp",
                table: "AvailableAbsence");

            migrationBuilder.DropForeignKey(
                name: "Contract_Employee",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "Request_Employee",
                table: "Request");

            migrationBuilder.AddForeignKey(
                name: "AvailableAbsence_Emp",
                table: "AvailableAbsence",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Contract_Employee",
                table: "Contract",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Request_Employee",
                table: "Request",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "AvailableAbsence_Emp",
                table: "AvailableAbsence");

            migrationBuilder.DropForeignKey(
                name: "Contract_Employee",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "Request_Employee",
                table: "Request");

            migrationBuilder.AddForeignKey(
                name: "AvailableAbsence_Emp",
                table: "AvailableAbsence",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Contract_Employee",
                table: "Contract",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "Request_Employee",
                table: "Request",
                column: "IdEmployee",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
