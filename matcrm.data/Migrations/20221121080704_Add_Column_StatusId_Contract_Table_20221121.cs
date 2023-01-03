using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_StatusId_Contract_Table_20221121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "Contract",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_StatusId",
                table: "Contract",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Status_StatusId",
                table: "Contract",
                column: "StatusId",
                principalSchema: "AppTask",
                principalTable: "Status",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Status_StatusId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_StatusId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Contract");
        }
    }
}
