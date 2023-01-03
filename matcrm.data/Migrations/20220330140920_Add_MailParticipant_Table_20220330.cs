using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_MailParticipant_Table_20220330 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailParticipant",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ThreadId = table.Column<string>(type: "text", nullable: true),
                    IntProviderAppSecretId = table.Column<long>(type: "bigint", nullable: true),
                    TeamMemberId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailParticipant_IntProviderAppSecret_IntProviderAppSecretId",
                        column: x => x.IntProviderAppSecretId,
                        principalTable: "IntProviderAppSecret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailParticipant_Users_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailParticipant_IntProviderAppSecretId",
                table: "MailParticipant",
                column: "IntProviderAppSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_MailParticipant_TeamMemberId",
                table: "MailParticipant",
                column: "TeamMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailParticipant");
        }
    }
}
