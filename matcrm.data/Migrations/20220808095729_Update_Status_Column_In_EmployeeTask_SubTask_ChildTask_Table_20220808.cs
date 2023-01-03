using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Update_Status_Column_In_EmployeeTask_SubTask_ChildTask_Table_20220808 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeChildTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeChildTask");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSubTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeSubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeChildTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeChildTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "Status",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSubTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeSubTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "Status",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "Status",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeChildTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeChildTask");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSubTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeSubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_Status_StatusId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeChildTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeChildTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "EmployeeTaskStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSubTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeSubTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "EmployeeTaskStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_EmployeeTaskStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "EmployeeTaskStatus",
                principalColumn: "Id");
        }
    }
}
