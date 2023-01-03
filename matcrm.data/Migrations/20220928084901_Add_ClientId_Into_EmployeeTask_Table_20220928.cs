using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientId_Into_EmployeeTask_Table_20220928 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ClientId",
                principalSchema: "AppCRM",
                principalTable: "Client",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_ClientId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeTask");
        }
    }
}
