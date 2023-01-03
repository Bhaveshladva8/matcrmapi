using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_MateTaskTimeRecord_And_MateSubTaskTimeRecord_Table_20221020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateChildTaskTimeRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateChildTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateChildTaskTimeRecord_EmployeeChildTask_ChildTaskId",
                        column: x => x.ChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateChildTaskTimeRecord_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MateSubTaskTimeRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateSubTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateSubTaskTimeRecord_EmployeeSubTask_SubTaskId",
                        column: x => x.SubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateSubTaskTimeRecord_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateChildTaskTimeRecord_ChildTaskId",
                table: "MateChildTaskTimeRecord",
                column: "ChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MateChildTaskTimeRecord_MateTimeRecordId",
                table: "MateChildTaskTimeRecord",
                column: "MateTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MateSubTaskTimeRecord_MateTimeRecordId",
                table: "MateSubTaskTimeRecord",
                column: "MateTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MateSubTaskTimeRecord_SubTaskId",
                table: "MateSubTaskTimeRecord",
                column: "SubTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateChildTaskTimeRecord");

            migrationBuilder.DropTable(
                name: "MateSubTaskTimeRecord");
        }
    }
}
