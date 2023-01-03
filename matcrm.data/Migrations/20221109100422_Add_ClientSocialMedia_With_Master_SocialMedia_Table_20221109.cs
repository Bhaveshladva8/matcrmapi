using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientSocialMedia_With_Master_SocialMedia_Table_20221109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialMedia",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMedia_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialMedia_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientSocialMedia",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    SocialMediaId = table.Column<long>(type: "bigint", nullable: true),
                    URL = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSocialMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSocialMedia_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientSocialMedia_SocialMedia_SocialMediaId",
                        column: x => x.SocialMediaId,
                        principalSchema: "AppCRM",
                        principalTable: "SocialMedia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientSocialMedia_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientSocialMedia_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientSocialMedia_ClientId",
                schema: "AppCRM",
                table: "ClientSocialMedia",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSocialMedia_CreatedBy",
                schema: "AppCRM",
                table: "ClientSocialMedia",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSocialMedia_SocialMediaId",
                schema: "AppCRM",
                table: "ClientSocialMedia",
                column: "SocialMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSocialMedia_UpdatedBy",
                schema: "AppCRM",
                table: "ClientSocialMedia",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedia_CreatedBy",
                schema: "AppCRM",
                table: "SocialMedia",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedia_UpdatedBy",
                schema: "AppCRM",
                table: "SocialMedia",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSocialMedia",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "SocialMedia",
                schema: "AppCRM");
        }
    }
}
