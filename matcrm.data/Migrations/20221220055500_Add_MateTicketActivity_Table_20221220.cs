using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTicketActivity_Table_20221220 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicketActivity",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activity = table.Column<string>(type: "text", nullable: true),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeProjectId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicketActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicketActivity_EmployeeProject_EmployeeProjectId",
                        column: x => x.EmployeeProjectId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeProject",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketActivity_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketActivity_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketActivity_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketActivity_CreatedBy",
                schema: "AppTask",
                table: "MateTicketActivity",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketActivity_EmployeeProjectId",
                schema: "AppTask",
                table: "MateTicketActivity",
                column: "EmployeeProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketActivity_EmployeeTaskId",
                schema: "AppTask",
                table: "MateTicketActivity",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketActivity_MateTicketId",
                schema: "AppTask",
                table: "MateTicketActivity",
                column: "MateTicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicketActivity",
                schema: "AppTask");
        }
    }
}
