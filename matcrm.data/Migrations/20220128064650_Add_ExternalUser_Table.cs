using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_ExternalUser_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "varchar(500)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(500)", nullable: true),
                    Id_Token = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", nullable: true),
                    Token_Type = table.Column<string>(type: "varchar(500)", nullable: true),
                    IntProviderId = table.Column<long>(type: "bigint", nullable: true),
                    ExpiredOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalUser_IntProvider_IntProviderId",
                        column: x => x.IntProviderId,
                        principalTable: "IntProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalUser_IntProviderId",
                table: "ExternalUser",
                column: "IntProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalUser");
        }
    }
}
