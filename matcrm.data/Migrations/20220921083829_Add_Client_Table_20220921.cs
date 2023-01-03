using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Client_Table_20220921 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FirstName = table.Column<string>(type: "varchar(500)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(500)", nullable: true),
                    OrganizationName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    SiteName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    SiteContactNumber = table.Column<string>(type: "varchar(100)", nullable: true),
                    SiteAddressLine1 = table.Column<string>(type: "varchar", nullable: true),
                    SiteAddressLine2 = table.Column<string>(type: "varchar", nullable: true),
                    SiteAddressLine3 = table.Column<string>(type: "varchar", nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(100)", nullable: true),
                    CountryId = table.Column<long>(type: "bigint", nullable: true),
                    StateId = table.Column<long>(type: "bigint", nullable: true),
                    CityId = table.Column<long>(type: "bigint", nullable: true),
                    TimeZoneId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Client_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Client_StandardTimeZone_TimeZoneId",
                        column: x => x.TimeZoneId,
                        principalTable: "StandardTimeZone",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Client_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Client_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_CityId",
                schema: "AppCRM",
                table: "Client",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_CountryId",
                schema: "AppCRM",
                table: "Client",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_StateId",
                schema: "AppCRM",
                table: "Client",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_TenantId",
                schema: "AppCRM",
                table: "Client",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_TimeZoneId",
                schema: "AppCRM",
                table: "Client",
                column: "TimeZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Client",
                schema: "AppCRM");
        }
    }
}
