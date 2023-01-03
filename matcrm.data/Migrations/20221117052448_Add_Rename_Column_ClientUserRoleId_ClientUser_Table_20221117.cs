using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Rename_Column_ClientUserRoleId_ClientUser_Table_20221117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_ClientUserRole_RoleId",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "ClientUserRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_RoleId",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "IX_ClientUser_ClientUserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_ClientUserRole_ClientUserRoleId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "ClientUserRoleId",
                principalSchema: "AppCRM",
                principalTable: "ClientUserRole",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_ClientUserRole_ClientUserRoleId",
                schema: "AppCRM",
                table: "ClientUser");

            migrationBuilder.RenameColumn(
                name: "ClientUserRoleId",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_ClientUserRoleId",
                schema: "AppCRM",
                table: "ClientUser",
                newName: "IX_ClientUser_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_ClientUserRole_RoleId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "RoleId",
                principalSchema: "AppCRM",
                principalTable: "ClientUserRole",
                principalColumn: "Id");
        }
    }
}
