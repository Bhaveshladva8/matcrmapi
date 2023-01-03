using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_Theme_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OneClappLatestThemeConfigId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OneClappLatestTheme",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    Accent = table.Column<string>(type: "varchar(500)", nullable: true),
                    Primary = table.Column<string>(type: "varchar(500)", nullable: true),
                    Warn = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappLatestTheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappLatestThemeLayout",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    TemplateHtml = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappLatestThemeLayout", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappLatestThemeScheme",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    TemplateHtml = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappLatestThemeScheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappLatestThemeConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OneClappLatestThemeId = table.Column<long>(type: "bigint", nullable: true),
                    OneClappLatestThemeLayoutId = table.Column<long>(type: "bigint", nullable: true),
                    OneClappLatestThemeSchemeId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappLatestThemeConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappLatestThemeConfig_OneClappLatestTheme_OneClappLates~",
                        column: x => x.OneClappLatestThemeId,
                        principalTable: "OneClappLatestTheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappLatestThemeConfig_OneClappLatestThemeLayout_OneClap~",
                        column: x => x.OneClappLatestThemeLayoutId,
                        principalTable: "OneClappLatestThemeLayout",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappLatestThemeConfig_OneClappLatestThemeScheme_OneClap~",
                        column: x => x.OneClappLatestThemeSchemeId,
                        principalTable: "OneClappLatestThemeScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_OneClappLatestThemeConfigId",
                table: "Users",
                column: "OneClappLatestThemeConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappLatestThemeConfig_OneClappLatestThemeId",
                table: "OneClappLatestThemeConfig",
                column: "OneClappLatestThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappLatestThemeConfig_OneClappLatestThemeLayoutId",
                table: "OneClappLatestThemeConfig",
                column: "OneClappLatestThemeLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappLatestThemeConfig_OneClappLatestThemeSchemeId",
                table: "OneClappLatestThemeConfig",
                column: "OneClappLatestThemeSchemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_OneClappLatestThemeConfig_OneClappLatestThemeConfigId",
                table: "Users",
                column: "OneClappLatestThemeConfigId",
                principalTable: "OneClappLatestThemeConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_OneClappLatestThemeConfig_OneClappLatestThemeConfigId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "OneClappLatestThemeConfig");

            migrationBuilder.DropTable(
                name: "OneClappLatestTheme");

            migrationBuilder.DropTable(
                name: "OneClappLatestThemeLayout");

            migrationBuilder.DropTable(
                name: "OneClappLatestThemeScheme");

            migrationBuilder.DropIndex(
                name: "IX_Users_OneClappLatestThemeConfigId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OneClappLatestThemeConfigId",
                table: "Users");
        }
    }
}
