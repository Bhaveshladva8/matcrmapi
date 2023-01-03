using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_ProjectCategoryId_In_EmployeeProject_Table_20221122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "ProjectCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_ProjectCategory_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "ProjectCategoryId",
                principalSchema: "AppTask",
                principalTable: "ProjectCategory",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_ProjectCategory_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProject_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject");
        }
    }
}
