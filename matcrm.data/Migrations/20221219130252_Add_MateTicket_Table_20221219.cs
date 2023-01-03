using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTicket_Table_20221219 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicket",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    MateCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    MatePriorityId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicket_MateCategory_MateCategoryId",
                        column: x => x.MateCategoryId,
                        principalSchema: "AppTask",
                        principalTable: "MateCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicket_MatePriority_MatePriorityId",
                        column: x => x.MatePriorityId,
                        principalSchema: "AppTask",
                        principalTable: "MatePriority",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicket_Status_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "AppTask",
                        principalTable: "Status",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicket_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicket_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicket_CreatedBy",
                schema: "AppTask",
                table: "MateTicket",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicket_MateCategoryId",
                schema: "AppTask",
                table: "MateTicket",
                column: "MateCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicket_MatePriorityId",
                schema: "AppTask",
                table: "MateTicket",
                column: "MatePriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicket_StatusId",
                schema: "AppTask",
                table: "MateTicket",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicket_UpdatedBy",
                schema: "AppTask",
                table: "MateTicket",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicket",
                schema: "AppTask");
        }
    }
}
