using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_Columns_in_Project_Section_And_EmplyeeTask_Table_20220628 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                schema: "AppTask",
                table: "Section",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SectionId",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedTime",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                schema: "AppTask",
                table: "EmployeeProject",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Section_ProjectId",
                schema: "AppTask",
                table: "Section",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_SectionId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_Section_SectionId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "SectionId",
                principalSchema: "AppTask",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Section_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "Section",
                column: "ProjectId",
                principalSchema: "AppTask",
                principalTable: "EmployeeProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_Section_SectionId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Section_EmployeeProject_ProjectId",
                schema: "AppTask",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Section_ProjectId",
                schema: "AppTask",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_SectionId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "AppTask",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "SectionId",
                schema: "AppTask",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "EstimatedTime",
                schema: "AppTask",
                table: "EmployeeProject");

            migrationBuilder.DropColumn(
                name: "Logo",
                schema: "AppTask",
                table: "EmployeeProject");
        }
    }
}
