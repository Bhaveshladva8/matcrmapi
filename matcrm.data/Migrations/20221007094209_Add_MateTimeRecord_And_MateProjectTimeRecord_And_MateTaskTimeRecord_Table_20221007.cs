using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTimeRecord_And_MateProjectTimeRecord_And_MateTaskTimeRecord_Table_20221007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    Comment = table.Column<string>(type: "varchar", nullable: true),
                    IsBillable = table.Column<bool>(type: "boolean", nullable: true),
                    ServiceArticleId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTimeRecord_ServiceArticle_ServiceArticleId",
                        column: x => x.ServiceArticleId,
                        principalSchema: "AppServiceArticle",
                        principalTable: "ServiceArticle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTimeRecord_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateProjectTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateProjectTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateProjectTimeRecord_EmployeeProject_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeProject",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateProjectTimeRecord_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateTaskTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTaskTimeRecord_EmployeeTask_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTaskTimeRecord_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateProjectTimeRecord_MateTimeRecordId",
                schema: "AppTask",
                table: "MateProjectTimeRecord",
                column: "MateTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MateProjectTimeRecord_ProjectId",
                schema: "AppTask",
                table: "MateProjectTimeRecord",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTaskTimeRecord_MateTimeRecordId",
                schema: "AppTask",
                table: "MateTaskTimeRecord",
                column: "MateTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTaskTimeRecord_TaskId",
                schema: "AppTask",
                table: "MateTaskTimeRecord",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTimeRecord_ServiceArticleId",
                schema: "AppTask",
                table: "MateTimeRecord",
                column: "ServiceArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTimeRecord_UserId",
                schema: "AppTask",
                table: "MateTimeRecord",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateProjectTimeRecord",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MateTaskTimeRecord",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MateTimeRecord",
                schema: "AppTask");
        }
    }
}
