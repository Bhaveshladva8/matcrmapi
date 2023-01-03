using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_ClientUserId_In_ClientAppointment_Table_20221111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientAppointment_ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment",
                column: "ClientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAppointment_ClientUser_ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment",
                column: "ClientUserId",
                principalSchema: "AppCRM",
                principalTable: "ClientUser",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientAppointment_ClientUser_ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment");

            migrationBuilder.DropIndex(
                name: "IX_ClientAppointment_ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                schema: "AppCRM",
                table: "ClientAppointment");
        }
    }
}
