using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Update_Status_Column_In_EmployeeProject_Table_20220808 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_EmployeeProjectStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "Status",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_EmployeeProjectStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "EmployeeProjectStatus",
                principalColumn: "Id");
        }
    }
}
