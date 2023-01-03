using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateCommentAttachment_Table_20221101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateCommentAttachment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MateCommentId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateCommentAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateCommentAttachment_MateComment_MateCommentId",
                        column: x => x.MateCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateCommentAttachment_MateCommentId",
                schema: "AppTask",
                table: "MateCommentAttachment",
                column: "MateCommentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateCommentAttachment",
                schema: "AppTask");
        }
    }
}
