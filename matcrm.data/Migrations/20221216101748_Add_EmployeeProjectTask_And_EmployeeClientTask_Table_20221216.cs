using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_EmployeeProjectTask_And_EmployeeClientTask_Table_20221216 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeClientTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeClientTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeClientTask_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeClientTask_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProjectTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeProjectId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProjectTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProjectTask_EmployeeProject_EmployeeProjectId",
                        column: x => x.EmployeeProjectId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeProject",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeProjectTask_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeClientTask_ClientId",
                schema: "AppTask",
                table: "EmployeeClientTask",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeClientTask_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeClientTask",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectTask_EmployeeProjectId",
                schema: "AppTask",
                table: "EmployeeProjectTask",
                column: "EmployeeProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectTask_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeProjectTask",
                column: "EmployeeTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeClientTask",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeProjectTask",
                schema: "AppTask");
        }
    }
}
