using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientId_Column_Into_EmployeeProject_Table_20221010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_ClientId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "ClientId",
                principalSchema: "AppCRM",
                principalTable: "Client",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProject_ClientId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeProject");
        }
    }
}
