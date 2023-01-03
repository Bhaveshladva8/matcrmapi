using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_InvoiceIntervalId_Column_InTo_Client_And_Contract_Table_20221018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceIntervalId",
                table: "Contract",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsContractBaseInvoice",
                schema: "AppCRM",
                table: "Client",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_InvoiceIntervalId",
                table: "Contract",
                column: "InvoiceIntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client",
                column: "InvoiceIntervalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_InvoiceInterval_InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client",
                column: "InvoiceIntervalId",
                principalTable: "InvoiceInterval",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_InvoiceInterval_InvoiceIntervalId",
                table: "Contract",
                column: "InvoiceIntervalId",
                principalTable: "InvoiceInterval",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_InvoiceInterval_InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_InvoiceInterval_InvoiceIntervalId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_InvoiceIntervalId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Client_InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "InvoiceIntervalId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "InvoiceIntervalId",
                schema: "AppCRM",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IsContractBaseInvoice",
                schema: "AppCRM",
                table: "Client");
        }
    }
}
