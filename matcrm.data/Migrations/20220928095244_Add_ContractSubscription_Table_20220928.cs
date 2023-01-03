using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ContractSubscription_Table_20220928 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractSubscription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceArticleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSubscription_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractSubscription_ServiceArticle_ServiceArticleId",
                        column: x => x.ServiceArticleId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubscription_ContractId",
                table: "ContractSubscription",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubscription_ServiceArticleId",
                table: "ContractSubscription",
                column: "ServiceArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractSubscription");
        }
    }
}
