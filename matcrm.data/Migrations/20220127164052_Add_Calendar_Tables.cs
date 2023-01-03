using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_Calendar_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarList",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarRepeatType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(250)", nullable: true),
                    CreatedBy = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarRepeatType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "varchar(500)", nullable: true),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    CalendarListId = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StartTime = table.Column<string>(type: "varchar(100)", nullable: true),
                    RepeatCount = table.Column<int>(type: "integer", nullable: true),
                    RepeatTypeId = table.Column<long>(type: "bigint", nullable: true),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarTask_CalendarList_CalendarListId",
                        column: x => x.CalendarListId,
                        principalTable: "CalendarList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarTask_CalendarRepeatType_RepeatTypeId",
                        column: x => x.RepeatTypeId,
                        principalTable: "CalendarRepeatType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarSubTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "varchar(500)", nullable: true),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    CalendarTaskId = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StartTime = table.Column<string>(type: "varchar(500)", nullable: true),
                    RepeatCount = table.Column<int>(type: "integer", nullable: true),
                    RepeatTypeId = table.Column<long>(type: "bigint", nullable: true),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSubTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarSubTask_CalendarRepeatType_RepeatTypeId",
                        column: x => x.RepeatTypeId,
                        principalTable: "CalendarRepeatType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarSubTask_CalendarTask_CalendarTaskId",
                        column: x => x.CalendarTaskId,
                        principalTable: "CalendarTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSubTask_CalendarTaskId",
                table: "CalendarSubTask",
                column: "CalendarTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSubTask_RepeatTypeId",
                table: "CalendarSubTask",
                column: "RepeatTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTask_CalendarListId",
                table: "CalendarTask",
                column: "CalendarListId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTask_RepeatTypeId",
                table: "CalendarTask",
                column: "RepeatTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarSubTask");

            migrationBuilder.DropTable(
                name: "CalendarTask");

            migrationBuilder.DropTable(
                name: "CalendarList");

            migrationBuilder.DropTable(
                name: "CalendarRepeatType");
        }
    }
}
