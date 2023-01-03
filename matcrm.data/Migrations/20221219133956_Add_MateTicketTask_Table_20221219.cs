using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTicketTask_Table_20221219 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicketTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicketTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicketTask_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketTask_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketTask_EmployeeTaskId",
                schema: "AppTask",
                table: "MateTicketTask",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketTask_MateTicketId",
                schema: "AppTask",
                table: "MateTicketTask",
                column: "MateTicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicketTask",
                schema: "AppTask");
        }
    }
}
