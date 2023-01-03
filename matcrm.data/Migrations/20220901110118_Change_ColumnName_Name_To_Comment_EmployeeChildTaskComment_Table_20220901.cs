using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Change_ColumnName_Name_To_Comment_EmployeeChildTaskComment_Table_20220901 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "AppTask",
                table: "EmployeeChildTaskComment",
                newName: "Comment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                schema: "AppTask",
                table: "EmployeeChildTaskComment",
                newName: "Name");
        }
    }
}
