using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientEmail_Table_20220921 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientEmail",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "varchar(254)", nullable: true),
                    EmailTypeId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEmail_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientEmail_EmailPhoneNoType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailPhoneNoType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmail_ClientId",
                schema: "AppCRM",
                table: "ClientEmail",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmail_EmailTypeId",
                schema: "AppCRM",
                table: "ClientEmail",
                column: "EmailTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEmail",
                schema: "AppCRM");
        }
    }
}
