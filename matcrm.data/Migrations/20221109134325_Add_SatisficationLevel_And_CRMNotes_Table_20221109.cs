using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_SatisficationLevel_And_CRMNotes_Table_20221109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SatisficationLevel",
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
                    table.PrimaryKey("PK_SatisficationLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SatisficationLevel_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SatisficationLevel_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CRMNotes",
                schema: "AppCRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ClientUserId = table.Column<long>(type: "bigint", nullable: true),
                    MateTimeRecordId = table.Column<long>(type: "bigint", nullable: true),
                    NextCallDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SatisficationLevelId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CRMNotes_ClientUser_ClientUserId",
                        column: x => x.ClientUserId,
                        principalSchema: "AppCRM",
                        principalTable: "ClientUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CRMNotes_MateTimeRecord_MateTimeRecordId",
                        column: x => x.MateTimeRecordId,
                        principalSchema: "AppTask",
                        principalTable: "MateTimeRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CRMNotes_SatisficationLevel_SatisficationLevelId",
                        column: x => x.SatisficationLevelId,
                        principalSchema: "AppCRM",
                        principalTable: "SatisficationLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CRMNotes_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CRMNotes_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_ClientUserId",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_CreatedBy",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_MateTimeRecordId",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "MateTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_SatisficationLevelId",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "SatisficationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMNotes_UpdatedBy",
                schema: "AppCRM",
                table: "CRMNotes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SatisficationLevel_CreatedBy",
                schema: "AppCRM",
                table: "SatisficationLevel",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SatisficationLevel_UpdatedBy",
                schema: "AppCRM",
                table: "SatisficationLevel",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRMNotes",
                schema: "AppCRM");

            migrationBuilder.DropTable(
                name: "SatisficationLevel",
                schema: "AppCRM");
        }
    }
}
