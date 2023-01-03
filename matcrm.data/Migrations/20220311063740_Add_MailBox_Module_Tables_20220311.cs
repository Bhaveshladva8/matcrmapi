using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_MailBox_Module_Tables_20220311 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailAssignUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamMemberId = table.Column<int>(type: "integer", nullable: true),
                    ThreadId = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAssignUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailAssignUser_Users_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailBoxComment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamMemberId = table.Column<int>(type: "integer", nullable: true),
                    ThreadId = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailBoxComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailBoxComment_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailBoxComment_Users_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailBoxTeam",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailBoxTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailBoxTeam_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailRead",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ThreadId = table.Column<string>(type: "text", nullable: true),
                    ReadBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailRead_Users_ReadBy",
                        column: x => x.ReadBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamInbox",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntProviderAppSecretId = table.Column<long>(type: "bigint", nullable: true),
                    MailBoxTeamId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamInbox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamInbox_IntProviderAppSecret_IntProviderAppSecretId",
                        column: x => x.IntProviderAppSecretId,
                        principalTable: "IntProviderAppSecret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamInbox_MailBoxTeam_MailBoxTeamId",
                        column: x => x.MailBoxTeamId,
                        principalTable: "MailBoxTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discussion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Topic = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    TeamInboxId = table.Column<long>(type: "bigint", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    AssignUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discussion_TeamInbox_TeamInboxId",
                        column: x => x.TeamInboxId,
                        principalTable: "TeamInbox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discussion_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discussion_Users_AssignUserId",
                        column: x => x.AssignUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionAttachment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DiscussionId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionAttachment_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionComment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamMemberId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DiscussionId = table.Column<long>(type: "bigint", nullable: true),
                    ReplyCommentId = table.Column<long>(type: "bigint", nullable: true),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    PinnedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionComment_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionComment_DiscussionComment_ReplyCommentId",
                        column: x => x.ReplyCommentId,
                        principalTable: "DiscussionComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionComment_Users_PinnedBy",
                        column: x => x.PinnedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionComment_Users_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionParticipant",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscussionId = table.Column<long>(type: "bigint", nullable: true),
                    TeamMemberId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionParticipant_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionParticipant_Users_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionRead",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscussionId = table.Column<long>(type: "bigint", nullable: true),
                    ReadBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionRead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionRead_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionRead_Users_ReadBy",
                        column: x => x.ReadBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discussion_AssignUserId",
                table: "Discussion",
                column: "AssignUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussion_TeamInboxId",
                table: "Discussion",
                column: "TeamInboxId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussion_TenantId",
                table: "Discussion",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionAttachment_DiscussionId",
                table: "DiscussionAttachment",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionComment_DiscussionId",
                table: "DiscussionComment",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionComment_PinnedBy",
                table: "DiscussionComment",
                column: "PinnedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionComment_ReplyCommentId",
                table: "DiscussionComment",
                column: "ReplyCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionComment_TeamMemberId",
                table: "DiscussionComment",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionParticipant_DiscussionId",
                table: "DiscussionParticipant",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionParticipant_TeamMemberId",
                table: "DiscussionParticipant",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionRead_DiscussionId",
                table: "DiscussionRead",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionRead_ReadBy",
                table: "DiscussionRead",
                column: "ReadBy");

            migrationBuilder.CreateIndex(
                name: "IX_MailAssignUser_TeamMemberId",
                table: "MailAssignUser",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBoxComment_TeamMemberId",
                table: "MailBoxComment",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBoxComment_TenantId",
                table: "MailBoxComment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBoxTeam_TenantId",
                table: "MailBoxTeam",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRead_ReadBy",
                table: "MailRead",
                column: "ReadBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInbox_IntProviderAppSecretId",
                table: "TeamInbox",
                column: "IntProviderAppSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInbox_MailBoxTeamId",
                table: "TeamInbox",
                column: "MailBoxTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionAttachment");

            migrationBuilder.DropTable(
                name: "DiscussionComment");

            migrationBuilder.DropTable(
                name: "DiscussionParticipant");

            migrationBuilder.DropTable(
                name: "DiscussionRead");

            migrationBuilder.DropTable(
                name: "MailAssignUser");

            migrationBuilder.DropTable(
                name: "MailBoxComment");

            migrationBuilder.DropTable(
                name: "MailRead");

            migrationBuilder.DropTable(
                name: "Discussion");

            migrationBuilder.DropTable(
                name: "TeamInbox");

            migrationBuilder.DropTable(
                name: "MailBoxTeam");
        }
    }
}
