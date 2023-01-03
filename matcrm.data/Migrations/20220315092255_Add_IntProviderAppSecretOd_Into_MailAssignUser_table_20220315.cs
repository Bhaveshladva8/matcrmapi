using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_IntProviderAppSecretOd_Into_MailAssignUser_table_20220315 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IntProviderAppSecretId",
                table: "MailAssignUser",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailAssignUser_IntProviderAppSecretId",
                table: "MailAssignUser",
                column: "IntProviderAppSecretId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailAssignUser_IntProviderAppSecret_IntProviderAppSecretId",
                table: "MailAssignUser",
                column: "IntProviderAppSecretId",
                principalTable: "IntProviderAppSecret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailAssignUser_IntProviderAppSecret_IntProviderAppSecretId",
                table: "MailAssignUser");

            migrationBuilder.DropIndex(
                name: "IX_MailAssignUser_IntProviderAppSecretId",
                table: "MailAssignUser");

            migrationBuilder.DropColumn(
                name: "IntProviderAppSecretId",
                table: "MailAssignUser");
        }
    }
}
