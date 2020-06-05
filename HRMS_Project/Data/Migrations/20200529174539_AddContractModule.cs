using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HRMS_Project.Data.Migrations
{
    public partial class AddContractModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Benefit",
                columns: table => new
                {
                    IdBenefit = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(6, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Benefit_pk", x => x.IdBenefit);
                });

            migrationBuilder.CreateTable(
                name: "ContractStatus",
                columns: table => new
                {
                    IdContractStatus = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ContractStatus_pk", x => x.IdContractStatus);
                });

            migrationBuilder.CreateTable(
                name: "ContractType",
                columns: table => new
                {
                    IdContractType = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractType = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ContractType_pk", x => x.IdContractType);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    IdContract = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractNumber = table.Column<int>(nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    ContractStart = table.Column<DateTime>(type: "date", nullable: false),
                    ContractEnd = table.Column<DateTime>(type: "date", nullable: false),
                    IdContractType = table.Column<int>(nullable: false),
                    IdContractStatus = table.Column<int>(nullable: false),
                    IdEmployee = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Contract_pk", x => x.IdContract);
                    table.ForeignKey(
                        name: "Contract_ContractStatus",
                        column: x => x.IdContractStatus,
                        principalTable: "ContractStatus",
                        principalColumn: "IdContractStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Contract_ContractType",
                        column: x => x.IdContractType,
                        principalTable: "ContractType",
                        principalColumn: "IdContractType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Contract_Employee",
                        column: x => x.IdEmployee,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractBenefit",
                columns: table => new
                {
                    IdBenefitContract = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBenefit = table.Column<int>(nullable: false),
                    IdContract = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    BenefitIdBenefit = table.Column<int>(nullable: true),
                    ContractIdContract = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ContractBenefit_pk", x => new { x.IdBenefitContract, x.IdBenefit, x.IdContract });
                    table.ForeignKey(
                        name: "FK_ContractBenefit_Benefit_BenefitIdBenefit",
                        column: x => x.BenefitIdBenefit,
                        principalTable: "Benefit",
                        principalColumn: "IdBenefit",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractBenefit_Contract_ContractIdContract",
                        column: x => x.ContractIdContract,
                        principalTable: "Contract",
                        principalColumn: "IdContract",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contract_IdContractStatus",
                table: "Contract",
                column: "IdContractStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_IdContractType",
                table: "Contract",
                column: "IdContractType");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_IdEmployee",
                table: "Contract",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBenefit_BenefitIdBenefit",
                table: "ContractBenefit",
                column: "BenefitIdBenefit");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBenefit_ContractIdContract",
                table: "ContractBenefit",
                column: "ContractIdContract");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractBenefit");

            migrationBuilder.DropTable(
                name: "Benefit");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "ContractStatus");

            migrationBuilder.DropTable(
                name: "ContractType");
        }
    }
}
