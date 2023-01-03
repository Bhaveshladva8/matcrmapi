using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_TeamInboxAccess_Table_20220404 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "TeamInbox",
                type: "varchar(250)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TeamInbox",
                type: "varchar(1000)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamInboxAccess",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamInboxId = table.Column<long>(type: "bigint", nullable: true),
                    TeamMateId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamInboxAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamInboxAccess_TeamInbox_TeamInboxId",
                        column: x => x.TeamInboxId,
                        principalTable: "TeamInbox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamInboxAccess_Users_TeamMateId",
                        column: x => x.TeamMateId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamInboxAccess_TeamInboxId",
                table: "TeamInboxAccess",
                column: "TeamInboxId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInboxAccess_TeamMateId",
                table: "TeamInboxAccess",
                column: "TeamMateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamInboxAccess");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "TeamInbox");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TeamInbox");
        }
    }
}
