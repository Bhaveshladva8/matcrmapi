using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_ClientId_In_CRMNotes_Table_20221122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "AppCRM",
                table: "CRMNotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_ClientId",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRMNotes_Client_ClientId",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "ClientId",
                principalSchema: "AppCRM",
                principalTable: "Client",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRMNotes_Client_ClientId",
                schema: "AppCRM",
                table: "CRMNotes");

            migrationBuilder.DropIndex(
                name: "IX_CRMNotes_ClientId",
                schema: "AppCRM",
                table: "CRMNotes");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "AppCRM",
                table: "CRMNotes");
        }
    }
}
