using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_PaymentId_Columns_InTo_Mollie_tables_20220602 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "MollieSubscription",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "MollieSubscription");
        }
    }
}
