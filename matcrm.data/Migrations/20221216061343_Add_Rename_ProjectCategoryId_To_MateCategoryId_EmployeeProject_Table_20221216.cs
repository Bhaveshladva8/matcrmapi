using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Rename_ProjectCategoryId_To_MateCategoryId_EmployeeProject_Table_20221216 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_ProjectCategory_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.RenameColumn(
                name: "ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                newName: "MateCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProject_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                newName: "IX_EmployeeProject_MateCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_MateCategory_MateCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "MateCategoryId",
                principalSchema: "AppTask",
                principalTable: "MateCategory",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_MateCategory_MateCategoryId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.RenameColumn(
                name: "MateCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                newName: "ProjectCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProject_MateCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                newName: "IX_EmployeeProject_ProjectCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_ProjectCategory_ProjectCategoryId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "ProjectCategoryId",
                principalSchema: "AppTask",
                principalTable: "ProjectCategory",
                principalColumn: "Id");
        }
    }
}
