using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_UnitPrice_And_User_ForeignKey_InTo_ServiceArticle_Table_20220928 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceArticle_Tenants_TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.DropIndex(
                name: "IX_ServiceArticle_TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.AddColumn<long>(
                name: "UnitPrice",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_CreatedBy",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceArticle_Users_CreatedBy",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceArticle_Users_CreatedBy",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.DropIndex(
                name: "IX_ServiceArticle_CreatedBy",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "AppServiceArticle",
                table: "ServiceArticle");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceArticle_TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceArticle_Tenants_TenantId",
                schema: "AppServiceArticle",
                table: "ServiceArticle",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId");
        }
    }
}
