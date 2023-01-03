using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_AssetsManufacturer_And_ContractAsset_Table_20221109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetsManufacturer",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetsManufacturer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetsManufacturer_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetsManufacturer_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractAsset",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: true),
                    ManufacturerId = table.Column<long>(type: "bigint", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    BuyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ServiceExpireDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractAsset_AssetsManufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalSchema: "AppCRM",
                        principalTable: "AssetsManufacturer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractAsset_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractAsset_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractAsset_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetsManufacturer_CreatedBy",
                schema: "AppCRM",
                table: "AssetsManufacturer",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssetsManufacturer_UpdatedBy",
                schema: "AppCRM",
                table: "AssetsManufacturer",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAsset_ContractId",
                schema: "AppCRM",
                table: "ContractAsset",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAsset_CreatedBy",
                schema: "AppCRM",
                table: "ContractAsset",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAsset_ManufacturerId",
                schema: "AppCRM",
                table: "ContractAsset",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAsset_UpdatedBy",
                schema: "AppCRM",
                table: "ContractAsset",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractAsset",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "AssetsManufacturer",
                schema: "AppCRM");
        }
    }
}
