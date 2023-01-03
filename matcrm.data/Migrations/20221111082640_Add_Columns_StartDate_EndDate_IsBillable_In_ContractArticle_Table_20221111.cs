using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Columns_StartDate_EndDate_IsBillable_In_ContractArticle_Table_20221111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ContractArticle",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBillable",
                table: "ContractArticle",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ContractArticle",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ContractArticle");

            migrationBuilder.DropColumn(
                name: "IsBillable",
                table: "ContractArticle");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ContractArticle");
        }
    }
}
