using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_CustomDomainEmailConfig_Table_20220505 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomDomainEmailConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    IMAPHost = table.Column<string>(type: "varchar(1000)", nullable: true),
                    IMAPPort = table.Column<int>(type: "integer", nullable: true),
                    SMTPHost = table.Column<string>(type: "varchar(1000)", nullable: true),
                    SMTPPort = table.Column<int>(type: "integer", nullable: true),
                    Email = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Password = table.Column<string>(type: "varchar(10000)", nullable: true),
                    IntProviderAppSecretId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomDomainEmailConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomDomainEmailConfig_IntProviderAppSecret_IntProviderApp~",
                        column: x => x.IntProviderAppSecretId,
                        principalTable: "IntProviderAppSecret",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomDomainEmailConfig_IntProviderAppSecretId",
                table: "CustomDomainEmailConfig",
                column: "IntProviderAppSecretId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomDomainEmailConfig");
        }
    }
}
