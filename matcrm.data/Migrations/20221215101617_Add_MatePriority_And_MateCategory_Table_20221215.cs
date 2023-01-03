using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MatePriority_And_MateCategory_Table_20221215 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateCategory",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "varchar(100)", nullable: true),
                    CustomTableId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateCategory_CustomTable_CustomTableId",
                        column: x => x.CustomTableId,
                        principalTable: "CustomTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateCategory_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateCategory_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatePriority",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    Color = table.Column<string>(type: "varchar(200)", nullable: true),
                    CustomTableId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatePriority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatePriority_CustomTable_CustomTableId",
                        column: x => x.CustomTableId,
                        principalTable: "CustomTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatePriority_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatePriority_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateCategory_CreatedBy",
                schema: "AppTask",
                table: "MateCategory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MateCategory_CustomTableId",
                schema: "AppTask",
                table: "MateCategory",
                column: "CustomTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MateCategory_UpdatedBy",
                schema: "AppTask",
                table: "MateCategory",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatePriority_CreatedBy",
                schema: "AppTask",
                table: "MatePriority",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatePriority_CustomTableId",
                schema: "AppTask",
                table: "MatePriority",
                column: "CustomTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MatePriority_UpdatedBy",
                schema: "AppTask",
                table: "MatePriority",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateCategory",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MatePriority",
                schema: "AppTask");
        }
    }
}
