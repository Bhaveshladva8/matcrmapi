using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Department_And_ClientUserRole_And_ClientUser_Table_20221109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientUserRole",
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
                    table.PrimaryKey("PK_ClientUserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUserRole_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUserRole_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Department",
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
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientUser",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    ReportTo = table.Column<long>(type: "bigint", nullable: true),
                    RoleId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UnSubscribe = table.Column<bool>(type: "boolean", nullable: false),
                    IntProviderId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUser_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "AppCRM",
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_ClientUser_ReportTo",
                        column: x => x.ReportTo,
                        principalSchema: "AppCRM",
                        principalTable: "ClientUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_ClientUserRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "AppCRM",
                        principalTable: "ClientUserRole",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "AppCRM",
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_IntProvider_IntProviderId",
                        column: x => x.IntProviderId,
                        principalTable: "IntProvider",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUser_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_ClientId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_CreatedBy",
                schema: "AppCRM",
                table: "ClientUser",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_DepartmentId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_IntProviderId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "IntProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_ReportTo",
                schema: "AppCRM",
                table: "ClientUser",
                column: "ReportTo");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_RoleId",
                schema: "AppCRM",
                table: "ClientUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_UpdatedBy",
                schema: "AppCRM",
                table: "ClientUser",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUserRole_CreatedBy",
                schema: "AppCRM",
                table: "ClientUserRole",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUserRole_UpdatedBy",
                schema: "AppCRM",
                table: "ClientUserRole",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedBy",
                schema: "AppCRM",
                table: "Department",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Department_UpdatedBy",
                schema: "AppCRM",
                table: "Department",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientUser",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "ClientUserRole",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "AppCRM");
        }
    }
}
