using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ServiceArticle_And_ServiceArticleHour_Table_20220908 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceArticle",
                schema: "AppServiceArticle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(10000)", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: true),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    TaxId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceArticle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceArticle_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceArticle_ServiceArticleCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticleCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceArticle_Tax_TaxId",
                        column: x => x.TaxId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "Tax",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceArticle_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceArticleHour",
                schema: "AppServiceArticle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitPrice = table.Column<long>(type: "bigint", nullable: true),
                    ServiceArticleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceArticleHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceArticleHour_ServiceArticle_ServiceArticleId",
                        column: x => x.ServiceArticleId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_CategoryId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_CurrencyId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_TaxId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticleHour_ServiceArticleId",
                schema: "AppServiceArticle",
                table: "ServiceArticleHour",
                column: "ServiceArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceArticleHour",
                schema: "AppServiceArticle");

            migrationBuilder.DropTable(
                name: "ServiceArticle",
                schema: "AppServiceArticle");
        }
    }
}
