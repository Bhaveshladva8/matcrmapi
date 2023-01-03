using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MatePriorityId_In_EmployeeTask_Table_20221219 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "MatePriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_MatePriority_MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "MatePriorityId",
                principalSchema: "AppTask",
                principalTable: "MatePriority",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_MatePriority_MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "MatePriorityId",
                schema: "AppTask",
                table: "EmployeeTask");
        }
    }
}
