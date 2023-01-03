using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Name_Column_EmployeeTask_Table_And_Schema_MateChildTaskTimeRecord_MateSubTaskTimeRecord_20221102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "MateSubTaskTimeRecord",
                newName: "MateSubTaskTimeRecord",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "MateChildTaskTimeRecord",
                newName: "MateChildTaskTimeRecord",
                newSchema: "AppTask");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.RenameTable(
                name: "MateSubTaskTimeRecord",
                schema: "AppTask",
                newName: "MateSubTaskTimeRecord");

            migrationBuilder.RenameTable(
                name: "MateChildTaskTimeRecord",
                schema: "AppTask",
                newName: "MateChildTaskTimeRecord");
        }
    }
}
