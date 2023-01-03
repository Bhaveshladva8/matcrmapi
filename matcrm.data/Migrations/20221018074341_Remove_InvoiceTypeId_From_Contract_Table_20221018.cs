using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Remove_InvoiceTypeId_From_Contract_Table_20221018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_InvoiceType_InvoiceTypeId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_InvoiceTypeId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "InvoiceTypeId",
                table: "Contract");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceTypeId",
                table: "Contract",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_InvoiceTypeId",
                table: "Contract",
                column: "InvoiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_InvoiceType_InvoiceTypeId",
                table: "Contract",
                column: "InvoiceTypeId",
                principalTable: "InvoiceType",
                principalColumn: "Id");
        }
    }
}
