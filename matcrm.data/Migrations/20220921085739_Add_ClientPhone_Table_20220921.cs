using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_ClientPhone_Table_20220921 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientPhone",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhoneNo = table.Column<string>(type: "varchar(100)", nullable: true),
                    PhoneNoTypeId = table.Column<long>(type: "bigint", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPhone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPhone_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientPhone_EmailPhoneNoType_PhoneNoTypeId",
                        column: x => x.PhoneNoTypeId,
                        principalTable: "EmailPhoneNoType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPhone_ClientId",
                schema: "AppCRM",
                table: "ClientPhone",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPhone_PhoneNoTypeId",
                schema: "AppCRM",
                table: "ClientPhone",
                column: "PhoneNoTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPhone",
                schema: "AppCRM");
        }
    }
}
