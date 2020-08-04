using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class Request : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestStatus",
                columns: table => new
                {
                    IdRequestStatus = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RequestStatus_pk", x => x.IdRequestStatus);
                });

            migrationBuilder.CreateTable(
                name: "RequestType",
                columns: table => new
                {
                    IdRequestType = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestType = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RequestType_pk", x => x.IdRequestType);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    IdRequest = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(type: "date", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    IdRequestType = table.Column<int>(nullable: false),
                    IdRequestStatus = table.Column<int>(nullable: false),
                    IdEmployee = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    EmployeeComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Request_pk", x => x.IdRequest);
                    table.ForeignKey(
                        name: "Request_Employee",
                        column: x => x.IdEmployee,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Request_RequestStatus",
                        column: x => x.IdRequestStatus,
                        principalTable: "RequestStatus",
                        principalColumn: "IdRequestStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Request_RequestType",
                        column: x => x.IdRequestType,
                        principalTable: "RequestType",
                        principalColumn: "IdRequestType",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_IdEmployee",
                table: "Request",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_Request_IdRequestStatus",
                table: "Request",
                column: "IdRequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Request_IdRequestType",
                table: "Request",
                column: "IdRequestType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "RequestStatus");

            migrationBuilder.DropTable(
                name: "RequestType");
        }
    }
}
