using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_ProjectStatus_And_Add_column_in_Project_Table_20220627 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeProjectStatus",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "varchar(200)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProjectStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_StatusId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProject_EmployeeProjectStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "EmployeeProjectStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProject_EmployeeProjectStatus_StatusId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropTable(
                name: "EmployeeProjectStatus",
                schema: "AppTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProject_StatusId",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "AppTask",
                table: "EmployeeProject");
        }
    }
}
