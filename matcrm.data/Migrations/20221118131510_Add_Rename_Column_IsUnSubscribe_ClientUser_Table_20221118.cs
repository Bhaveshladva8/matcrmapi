using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Rename_Column_IsUnSubscribe_ClientUser_Table_20221118 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnSubscribe",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "IsUnSubscribe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsUnSubscribe",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "UnSubscribe");
        }
    }
}
