using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Remove_FormModule_Unnecessary_Tables_20220712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Border");

            migrationBuilder.DropTable(
                name: "BoxShadow");

            migrationBuilder.DropTable(
                name: "OneClappFormFieldStyle");

            migrationBuilder.DropTable(
                name: "OneClappFormStyle");

            migrationBuilder.DropTable(
                name: "Typography");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Border",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BorderStyleId = table.Column<long>(type: "bigint", nullable: true),
                    BorderBottom = table.Column<long>(type: "bigint", nullable: true),
                    BorderLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusBottomLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusBottomRight = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusTopLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusTopRight = table.Column<long>(type: "bigint", nullable: true),
                    BorderRight = table.Column<long>(type: "bigint", nullable: true),
                    BorderTop = table.Column<long>(type: "bigint", nullable: true),
                    Color = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Border", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Border_BorderStyle_BorderStyleId",
                        column: x => x.BorderStyleId,
                        principalTable: "BorderStyle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BoxShadow",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Blur = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Horizontal = table.Column<long>(type: "bigint", nullable: true),
                    Position = table.Column<string>(type: "varchar(500)", nullable: true),
                    Spread = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Vertical = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxShadow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormFieldStyle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MarginBottom = table.Column<long>(type: "bigint", nullable: true),
                    MarginTop = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Width = table.Column<string>(type: "varchar(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormFieldStyle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormStyle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Width = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormStyle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Typography",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FontFamily = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FontSize = table.Column<long>(type: "bigint", nullable: true),
                    FontStyle = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FontWeight = table.Column<long>(type: "bigint", nullable: true),
                    LetterSpacing = table.Column<long>(type: "bigint", nullable: true),
                    LineHeight = table.Column<long>(type: "bigint", nullable: true),
                    TextColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    TextStyle = table.Column<string>(type: "varchar(1000)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Typography", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Border_BorderStyleId",
                table: "Border",
                column: "BorderStyleId");
        }
    }
}
