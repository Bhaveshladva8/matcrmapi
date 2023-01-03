using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Change_ForeignKey_In_EmployeeProjectActivity_20220812 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjectActivity_Project_ProjectId",
                schema: "AppTask",
                table: "EmployeeProjectActivity");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjectActivity_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "EmployeeProjectActivity",
                column: "ProjectId",
                principalSchema: "AppTask",
                principalTable: "EmployeeProject",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjectActivity_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "EmployeeProjectActivity");

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjectActivity_Project_ProjectId",
                schema: "AppTask",
                table: "EmployeeProjectActivity",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }
    }
}
