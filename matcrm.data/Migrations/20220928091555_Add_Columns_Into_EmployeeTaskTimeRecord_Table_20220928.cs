using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Columns_Into_EmployeeTaskTimeRecord_Table_20220928 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBillable",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskTimeRecord_ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                column: "ServiceArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTaskTimeRecord_ServiceArticle_ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                column: "ServiceArticleId",
                principalSchema: "AppServiceArticle",
                principalTable: "ServiceArticle",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTaskTimeRecord_ServiceArticle_ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTaskTimeRecord_ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord");

            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord");

            migrationBuilder.DropColumn(
                name: "IsBillable",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord");

            migrationBuilder.DropColumn(
                name: "ServiceArticleId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord");
        }
    }
}
