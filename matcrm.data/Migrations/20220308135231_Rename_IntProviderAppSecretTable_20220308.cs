using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Rename_IntProviderAppSecretTable_20220308 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarSyncActivity_IntProviderAppSercret_IntProviderAppSe~",
                table: "CalendarSyncActivity");

            migrationBuilder.DropTable(
                name: "IntProviderAppSercret");

            migrationBuilder.RenameColumn(
                name: "IntProviderAppSercretId",
                table: "CalendarSyncActivity",
                newName: "IntProviderAppSecretId");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarSyncActivity_IntProviderAppSercretId",
                table: "CalendarSyncActivity",
                newName: "IX_CalendarSyncActivity_IntProviderAppSecretId");

            migrationBuilder.CreateTable(
                name: "IntProviderAppSecret",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Access_Token = table.Column<string>(type: "text", nullable: true),
                    Expires_In = table.Column<long>(type: "bigint", nullable: true),
                    Refresh_Token = table.Column<string>(type: "text", nullable: true),
                    Scope = table.Column<string>(type: "text", nullable: true),
                    Token_Type = table.Column<string>(type: "varchar(500)", nullable: true),
                    Id_Token = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", nullable: true),
                    IntProviderAppId = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsSelect = table.Column<bool>(type: "boolean", nullable: false),
                    LastAccessedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntProviderAppSecret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntProviderAppSecret_IntProviderApp_IntProviderAppId",
                        column: x => x.IntProviderAppId,
                        principalTable: "IntProviderApp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderAppSecret_IntProviderAppId",
                table: "IntProviderAppSecret",
                column: "IntProviderAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarSyncActivity_IntProviderAppSecret_IntProviderAppSec~",
                table: "CalendarSyncActivity",
                column: "IntProviderAppSecretId",
                principalTable: "IntProviderAppSecret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarSyncActivity_IntProviderAppSecret_IntProviderAppSec~",
                table: "CalendarSyncActivity");

            migrationBuilder.DropTable(
                name: "IntProviderAppSecret");

            migrationBuilder.RenameColumn(
                name: "IntProviderAppSecretId",
                table: "CalendarSyncActivity",
                newName: "IntProviderAppSercretId");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarSyncActivity_IntProviderAppSecretId",
                table: "CalendarSyncActivity",
                newName: "IX_CalendarSyncActivity_IntProviderAppSercretId");

            migrationBuilder.CreateTable(
                name: "IntProviderAppSercret",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Access_Token = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", nullable: true),
                    Expires_In = table.Column<long>(type: "bigint", nullable: true),
                    Id_Token = table.Column<string>(type: "text", nullable: true),
                    IntProviderAppId = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSelect = table.Column<bool>(type: "boolean", nullable: false),
                    LastAccessedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Refresh_Token = table.Column<string>(type: "text", nullable: true),
                    Scope = table.Column<string>(type: "text", nullable: true),
                    Token_Type = table.Column<string>(type: "varchar(500)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntProviderAppSercret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntProviderAppSercret_IntProviderApp_IntProviderAppId",
                        column: x => x.IntProviderAppId,
                        principalTable: "IntProviderApp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderAppSercret_IntProviderAppId",
                table: "IntProviderAppSercret",
                column: "IntProviderAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarSyncActivity_IntProviderAppSercret_IntProviderAppSe~",
                table: "CalendarSyncActivity",
                column: "IntProviderAppSercretId",
                principalTable: "IntProviderAppSercret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
