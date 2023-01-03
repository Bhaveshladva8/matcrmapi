using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_CustomerId_InTo_Discussion_Table_20220412 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "Discussion",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussion_CustomerId",
                table: "Discussion",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussion_Customer_CustomerId",
                table: "Discussion",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussion_Customer_CustomerId",
                table: "Discussion");

            migrationBuilder.DropIndex(
                name: "IX_Discussion_CustomerId",
                table: "Discussion");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Discussion");
        }
    }
}
