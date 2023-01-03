using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Rename_IsFinalize_Column_To_IsDefault_Status_Table_20220808 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFinalize",
                schema: "AppTask",
                table: "Status",
                newName: "IsDefault");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDefault",
                schema: "AppTask",
                table: "Status",
                newName: "IsFinalize");
        }
    }
}
