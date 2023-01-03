using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateComment_Table_20221101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    MateReplyCommentId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateComment_MateComment_MateReplyCommentId",
                        column: x => x.MateReplyCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateComment_MateReplyCommentId",
                schema: "AppTask",
                table: "MateComment",
                column: "MateReplyCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MateComment_UserId",
                schema: "AppTask",
                table: "MateComment",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateComment",
                schema: "AppTask");
        }
    }
}
