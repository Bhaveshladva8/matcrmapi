using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    /// <inheritdoc />
    public partial class AddMateTicketCommentTable20221222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTicketComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateTicketId = table.Column<long>(type: "bigint", nullable: true),
                    MateCommentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTicketComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTicketComment_MateComment_MateCommentId",
                        column: x => x.MateCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTicketComment_MateTicket_MateTicketId",
                        column: x => x.MateTicketId,
                        principalSchema: "AppTask",
                        principalTable: "MateTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketComment_MateCommentId",
                schema: "AppTask",
                table: "MateTicketComment",
                column: "MateCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTicketComment_MateTicketId",
                schema: "AppTask",
                table: "MateTicketComment",
                column: "MateTicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateTicketComment",
                schema: "AppTask");
        }
    }
}
