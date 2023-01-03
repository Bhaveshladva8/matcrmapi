using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_SalutationId_In_ClientUser_Table_20221117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SalutationId",
                schema: "AppCRM",
                table: "ClientUser",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_SalutationId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "SalutationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_Salutation_SalutationId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "SalutationId",
                principalTable: "Salutation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Salutation_SalutationId",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropIndex(
                name: "IX_ClientUser_SalutationId",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropColumn(
                name: "SalutationId",
                schema: "AppCRM",
                table: "ClientUser");
        }
    }
}
