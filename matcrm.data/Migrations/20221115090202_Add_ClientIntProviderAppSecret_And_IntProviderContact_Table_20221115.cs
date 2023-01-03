using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientIntProviderAppSecret_And_IntProviderContact_Table_20221115 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientIntProviderAppSecret",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Access_Token = table.Column<string>(type: "text", nullable: true),
                    Expires_In = table.Column<long>(type: "bigint", nullable: true),
                    Refresh_Token = table.Column<string>(type: "text", nullable: true),
                    Scope = table.Column<string>(type: "text", nullable: true),
                    Token_Type = table.Column<string>(type: "text", nullable: true),
                    Id_Token = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IntProviderAppId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    LastAccessedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoggedInUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientIntProviderAppSecret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientIntProviderAppSecret_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientIntProviderAppSecret_IntProviderApp_IntProviderAppId",
                        column: x => x.IntProviderAppId,
                        principalTable: "IntProviderApp",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientIntProviderAppSecret_Users_LoggedInUserId",
                        column: x => x.LoggedInUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientIntProviderAppSecret_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IntProviderContact",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactId = table.Column<string>(type: "text", nullable: true),
                    ClientIntProviderAppSecretId = table.Column<long>(type: "bigint", nullable: true),
                    ClientUserId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    LoggedInUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntProviderContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntProviderContact_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IntProviderContact_ClientIntProviderAppSecret_ClientIntProv~",
                        column: x => x.ClientIntProviderAppSecretId,
                        principalSchema: "AppCRM",
                        principalTable: "ClientIntProviderAppSecret",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IntProviderContact_ClientUser_ClientUserId",
                        column: x => x.ClientUserId,
                        principalSchema: "AppCRM",
                        principalTable: "ClientUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IntProviderContact_Users_LoggedInUserId",
                        column: x => x.LoggedInUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IntProviderContact_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientIntProviderAppSecret_ClientId",
                schema: "AppCRM",
                table: "ClientIntProviderAppSecret",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientIntProviderAppSecret_IntProviderAppId",
                schema: "AppCRM",
                table: "ClientIntProviderAppSecret",
                column: "IntProviderAppId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientIntProviderAppSecret_LoggedInUserId",
                schema: "AppCRM",
                table: "ClientIntProviderAppSecret",
                column: "LoggedInUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientIntProviderAppSecret_UpdatedBy",
                schema: "AppCRM",
                table: "ClientIntProviderAppSecret",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderContact_ClientId",
                schema: "AppCRM",
                table: "IntProviderContact",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderContact_ClientIntProviderAppSecretId",
                schema: "AppCRM",
                table: "IntProviderContact",
                column: "ClientIntProviderAppSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderContact_ClientUserId",
                schema: "AppCRM",
                table: "IntProviderContact",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderContact_LoggedInUserId",
                schema: "AppCRM",
                table: "IntProviderContact",
                column: "LoggedInUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IntProviderContact_UpdatedBy",
                schema: "AppCRM",
                table: "IntProviderContact",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntProviderContact",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "ClientIntProviderAppSecret",
                schema: "AppCRM");
        }
    }
}
