using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateClientTicket_MateProjectTicket_Table_20221219 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateClientTicket",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateClientTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateClientTicket_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateClientTicket_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateProjectTicket",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeProjectId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateProjectTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateProjectTicket_EmployeeProject_EmployeeProjectId",
                        column: x => x.EmployeeProjectId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeProject",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateProjectTicket_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateClientTicket_ClientId",
                schema: "AppTask",
                table: "MateClientTicket",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MateClientTicket_MateTicketId",
                schema: "AppTask",
                table: "MateClientTicket",
                column: "MateTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_MateProjectTicket_EmployeeProjectId",
                schema: "AppTask",
                table: "MateProjectTicket",
                column: "EmployeeProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MateProjectTicket_MateTicketId",
                schema: "AppTask",
                table: "MateProjectTicket",
                column: "MateTicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateClientTicket",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MateProjectTicket",
                schema: "AppTask");
        }
    }
}
