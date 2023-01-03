using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTicketUser_Table_20221220 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicketUser",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicketUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicketUser_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketUser_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketUser_CreatedBy",
                schema: "AppTask",
                table: "MateTicketUser",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketUser_MateTicketId",
                schema: "AppTask",
                table: "MateTicketUser",
                column: "MateTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketUser_UserId",
                schema: "AppTask",
                table: "MateTicketUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicketUser",
                schema: "AppTask");
        }
    }
}
