using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ForeignKey_For_ChildTaskId_MateChildTaskTimeRecord_Table_20221101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MateChildTaskTimeRecord_ChildTaskId",
                table: "MateChildTaskTimeRecord",
                column: "ChildTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_MateChildTaskTimeRecord_EmployeeChildTask_ChildTaskId",
                table: "MateChildTaskTimeRecord",
                column: "ChildTaskId",
                principalSchema: "AppTask",
                principalTable: "EmployeeChildTask",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MateChildTaskTimeRecord_EmployeeChildTask_ChildTaskId",
                table: "MateChildTaskTimeRecord");

            migrationBuilder.DropIndex(
                name: "IX_MateChildTaskTimeRecord_ChildTaskId",
                table: "MateChildTaskTimeRecord");
        }
    }
}
