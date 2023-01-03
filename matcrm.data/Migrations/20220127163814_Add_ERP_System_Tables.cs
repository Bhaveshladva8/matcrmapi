using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_ERP_System_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ERPSystem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ERPSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserERPSystem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "varchar(254)", nullable: true),
                    AuthKey = table.Column<string>(type: "text", nullable: true),
                    Tenant = table.Column<string>(type: "text", nullable: true),
                    ERPId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserERPSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ERPSystemColumnMap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    WeClappUserId = table.Column<long>(type: "bigint", nullable: true),
                    ERPSystemId = table.Column<long>(type: "bigint", nullable: true),
                    SourceColumnName = table.Column<string>(type: "text", nullable: true),
                    DestinationColumnName = table.Column<string>(type: "text", nullable: true),
                    CustomModuleId = table.Column<long>(type: "bigint", nullable: true),
                    CustomFieldId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ERPSystemColumnMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ERPSystemColumnMap_CustomField_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ERPSystemColumnMap_CustomModule_CustomModuleId",
                        column: x => x.CustomModuleId,
                        principalTable: "CustomModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ERPSystemColumnMap_ERPSystem_ERPSystemId",
                        column: x => x.ERPSystemId,
                        principalTable: "ERPSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ERPSystemColumnMap_WeClappUser_WeClappUserId",
                        column: x => x.WeClappUserId,
                        principalTable: "WeClappUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ERPSystemColumnMap_CustomFieldId",
                table: "ERPSystemColumnMap",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ERPSystemColumnMap_CustomModuleId",
                table: "ERPSystemColumnMap",
                column: "CustomModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ERPSystemColumnMap_ERPSystemId",
                table: "ERPSystemColumnMap",
                column: "ERPSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ERPSystemColumnMap_WeClappUserId",
                table: "ERPSystemColumnMap",
                column: "WeClappUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ERPSystemColumnMap");

            migrationBuilder.DropTable(
                name: "UserERPSystem");

            migrationBuilder.DropTable(
                name: "ERPSystem");
        }
    }
}
