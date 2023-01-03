using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTaskComment_MateSubTaskComment_MateChildTaskComment_Table_20221101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateChildTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateCommentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateChildTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateChildTaskComment_EmployeeChildTask_ChildTaskId",
                        column: x => x.ChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateChildTaskComment_MateComment_MateCommentId",
                        column: x => x.MateCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateSubTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateCommentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateSubTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateSubTaskComment_EmployeeSubTask_SubTaskId",
                        column: x => x.SubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateSubTaskComment_MateComment_MateCommentId",
                        column: x => x.MateCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateCommentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateTaskComment_EmployeeTask_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateTaskComment_MateComment_MateCommentId",
                        column: x => x.MateCommentId,
                        principalSchema: "AppTask",
                        principalTable: "MateComment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateChildTaskComment_ChildTaskId",
                schema: "AppTask",
                table: "MateChildTaskComment",
                column: "ChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateChildTaskComment_MateCommentId",
                schema: "AppTask",
                table: "MateChildTaskComment",
                column: "MateCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MateSubTaskComment_MateCommentId",
                schema: "AppTask",
                table: "MateSubTaskComment",
                column: "MateCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MateSubTaskComment_SubTaskId",
                schema: "AppTask",
                table: "MateSubTaskComment",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTaskComment_MateCommentId",
                schema: "AppTask",
                table: "MateTaskComment",
                column: "MateCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_MateTaskComment_TaskId",
                schema: "AppTask",
                table: "MateTaskComment",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateChildTaskComment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MateSubTaskComment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "MateTaskComment",
                schema: "AppTask");
        }
    }
}
