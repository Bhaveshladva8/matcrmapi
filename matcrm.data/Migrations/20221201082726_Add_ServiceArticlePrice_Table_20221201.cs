using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ServiceArticlePrice_Table_20221201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceArticlePrice",
                schema: "AppServiceArticle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceArticleId = table.Column<long>(type: "bigint", nullable: true),
                    Price = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsBillable = table.Column<bool>(type: "boolean", nullable: true),
                    LoggedInUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceArticlePrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceArticlePrice_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceArticlePrice_ServiceArticle_ServiceArticleId",
                        column: x => x.ServiceArticleId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceArticlePrice_Users_LoggedInUserId",
                        column: x => x.LoggedInUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticlePrice_ClientId",
                schema: "AppServiceArticle",
                table: "ServiceArticlePrice",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticlePrice_LoggedInUserId",
                schema: "AppServiceArticle",
                table: "ServiceArticlePrice",
                column: "LoggedInUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticlePrice_ServiceArticleId",
                schema: "AppServiceArticle",
                table: "ServiceArticlePrice",
                column: "ServiceArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceArticlePrice",
                schema: "AppServiceArticle");
        }
    }
}
