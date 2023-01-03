using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ContractArticle_Table_20221018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractArticle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceArticleId = table.Column<long>(type: "bigint", nullable: true),
                    IsContractUnitPrice = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractArticle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractArticle_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractArticle_ServiceArticle_ServiceArticleId",
                        column: x => x.ServiceArticleId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractArticle_ContractId",
                table: "ContractArticle",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractArticle_ServiceArticleId",
                table: "ContractArticle",
                column: "ServiceArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractArticle");
        }
    }
}
