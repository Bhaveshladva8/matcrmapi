using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Schema_In_MailModule_20220712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppMail");

            migrationBuilder.RenameTable(
                name: "TeamInboxAccess",
                newName: "TeamInboxAccess",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "TeamInbox",
                newName: "TeamInbox",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailRead",
                newName: "MailRead",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailParticipant",
                newName: "MailParticipant",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailCommentAttachment",
                newName: "MailCommentAttachment",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailBoxTeam",
                newName: "MailBoxTeam",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailBoxComment",
                newName: "MailBoxComment",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailAssignUser",
                newName: "MailAssignUser",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "MailAssignCustomer",
                newName: "MailAssignCustomer",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "DiscussionRead",
                newName: "DiscussionRead",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "DiscussionParticipant",
                newName: "DiscussionParticipant",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "DiscussionCommentAttachment",
                newName: "DiscussionCommentAttachment",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "DiscussionComment",
                newName: "DiscussionComment",
                newSchema: "AppMail");

            migrationBuilder.RenameTable(
                name: "Discussion",
                newName: "Discussion",
                newSchema: "AppMail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TeamInboxAccess",
                schema: "AppMail",
                newName: "TeamInboxAccess");

            migrationBuilder.RenameTable(
                name: "TeamInbox",
                schema: "AppMail",
                newName: "TeamInbox");

            migrationBuilder.RenameTable(
                name: "MailRead",
                schema: "AppMail",
                newName: "MailRead");

            migrationBuilder.RenameTable(
                name: "MailParticipant",
                schema: "AppMail",
                newName: "MailParticipant");

            migrationBuilder.RenameTable(
                name: "MailCommentAttachment",
                schema: "AppMail",
                newName: "MailCommentAttachment");

            migrationBuilder.RenameTable(
                name: "MailBoxTeam",
                schema: "AppMail",
                newName: "MailBoxTeam");

            migrationBuilder.RenameTable(
                name: "MailBoxComment",
                schema: "AppMail",
                newName: "MailBoxComment");

            migrationBuilder.RenameTable(
                name: "MailAssignUser",
                schema: "AppMail",
                newName: "MailAssignUser");

            migrationBuilder.RenameTable(
                name: "MailAssignCustomer",
                schema: "AppMail",
                newName: "MailAssignCustomer");

            migrationBuilder.RenameTable(
                name: "DiscussionRead",
                schema: "AppMail",
                newName: "DiscussionRead");

            migrationBuilder.RenameTable(
                name: "DiscussionParticipant",
                schema: "AppMail",
                newName: "DiscussionParticipant");

            migrationBuilder.RenameTable(
                name: "DiscussionCommentAttachment",
                schema: "AppMail",
                newName: "DiscussionCommentAttachment");

            migrationBuilder.RenameTable(
                name: "DiscussionComment",
                schema: "AppMail",
                newName: "DiscussionComment");

            migrationBuilder.RenameTable(
                name: "Discussion",
                schema: "AppMail",
                newName: "Discussion");
        }
    }
}
