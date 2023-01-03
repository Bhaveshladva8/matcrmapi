using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTicketTimeRecord_Table_20221220 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicketTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicketTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicketTimeRecord_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketTimeRecord_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketTimeRecord_MateTicketId",
                schema: "AppTask",
                table: "MateTicketTimeRecord",
                column: "MateTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketTimeRecord_MateTimeRecordId",
                schema: "AppTask",
                table: "MateTicketTimeRecord",
                column: "MateTimeRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicketTimeRecord",
                schema: "AppTask");
        }
    }
}
