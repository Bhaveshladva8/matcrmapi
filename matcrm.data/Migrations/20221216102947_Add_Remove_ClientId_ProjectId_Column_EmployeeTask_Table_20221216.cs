using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Remove_ClientId_ProjectId_Column_EmployeeTask_Table_20221216 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_ClientId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_ProjectId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "AppTask",
                table: "EmployeeTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_ProjectId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_Client_ClientId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ClientId",
                principalSchema: "AppCRM",
                principalTable: "Client",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ProjectId",
                principalSchema: "AppTask",
                principalTable: "EmployeeProject",
                principalColumn: "Id");
        }
    }
}
