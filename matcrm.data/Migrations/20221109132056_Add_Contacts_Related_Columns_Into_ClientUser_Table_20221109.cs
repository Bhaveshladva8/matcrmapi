using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Contacts_Related_Columns_Into_ClientUser_Table_20221109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalEmail",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateTelephoneNo",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkEmail",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkTelephoneNo",
                schema: "AppCRM",
                table: "ClientUser",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileNo",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropColumn(
                name: "PersonalEmail",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropColumn(
                name: "PrivateTelephoneNo",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropColumn(
                name: "WorkEmail",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.DropColumn(
                name: "WorkTelephoneNo",
                schema: "AppCRM",
                table: "ClientUser");
        }
    }
}
