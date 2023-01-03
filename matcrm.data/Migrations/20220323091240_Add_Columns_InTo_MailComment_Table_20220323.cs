using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_Columns_InTo_MailComment_Table_20220323 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "MailBoxComment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "MailReplyCommentId",
                table: "MailBoxComment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PinnedBy",
                table: "MailBoxComment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailBoxComment_MailReplyCommentId",
                table: "MailBoxComment",
                column: "MailReplyCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBoxComment_PinnedBy",
                table: "MailBoxComment",
                column: "PinnedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_MailBoxComment_MailBoxComment_MailReplyCommentId",
                table: "MailBoxComment",
                column: "MailReplyCommentId",
                principalTable: "MailBoxComment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MailBoxComment_Users_PinnedBy",
                table: "MailBoxComment",
                column: "PinnedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailBoxComment_MailBoxComment_MailReplyCommentId",
                table: "MailBoxComment");

            migrationBuilder.DropForeignKey(
                name: "FK_MailBoxComment_Users_PinnedBy",
                table: "MailBoxComment");

            migrationBuilder.DropIndex(
                name: "IX_MailBoxComment_MailReplyCommentId",
                table: "MailBoxComment");

            migrationBuilder.DropIndex(
                name: "IX_MailBoxComment_PinnedBy",
                table: "MailBoxComment");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "MailBoxComment");

            migrationBuilder.DropColumn(
                name: "MailReplyCommentId",
                table: "MailBoxComment");

            migrationBuilder.DropColumn(
                name: "PinnedBy",
                table: "MailBoxComment");
        }
    }
}
