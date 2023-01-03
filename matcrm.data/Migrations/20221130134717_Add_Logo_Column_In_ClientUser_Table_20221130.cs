using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Logo_Column_In_ClientUser_Table_20221130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                schema: "AppCRM",
                table: "ClientUser");
        }
    }
}
