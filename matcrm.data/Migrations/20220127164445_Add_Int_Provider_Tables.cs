using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_Int_Provider_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntProvider",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntProviderApp",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    IntProviderId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntProviderApp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntProviderApp_IntProvider_IntProviderId",
                        column: x => x.IntProviderId,
                        principalTable: "IntProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntProviderAppSercret",
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
                    table.PrimaryKey("PK_IntProviderAppSercret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntProviderAppSercret_IntProviderApp_IntProviderAppId",
                        column: x => x.IntProviderAppId,
                        principalTable: "IntProviderApp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarSyncActivity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CalendarEventId = table.Column<string>(type: "text", nullable: true),
                    IntProviderAppSercretId = table.Column<long>(type: "bigint", nullable: true),
                    ActivityId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    ModuleId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSyncActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarSyncActivity_IntProviderAppSercret_IntProviderAppSe~",
                        column: x => x.IntProviderAppSercretId,
                        principalTable: "IntProviderAppSercret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarSyncActivity_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarSyncActivity_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSyncActivity_CreatedBy",
                table: "CalendarSyncActivity",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSyncActivity_IntProviderAppSercretId",
                table: "CalendarSyncActivity",
                column: "IntProviderAppSercretId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSyncActivity_TenantId",
                table: "CalendarSyncActivity",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderApp_IntProviderId",
                table: "IntProviderApp",
                column: "IntProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderAppSercret_IntProviderAppId",
                table: "IntProviderAppSercret",
                column: "IntProviderAppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarSyncActivity");

            migrationBuilder.DropTable(
                name: "IntProviderAppSercret");

            migrationBuilder.DropTable(
                name: "IntProviderApp");

            migrationBuilder.DropTable(
                name: "IntProvider");
        }
    }
}
