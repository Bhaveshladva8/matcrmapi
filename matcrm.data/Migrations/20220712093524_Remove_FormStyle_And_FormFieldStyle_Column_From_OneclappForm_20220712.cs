using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Remove_FormStyle_And_FormFieldStyle_Column_From_OneclappForm_20220712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OneClappForm_OneClappFormFieldStyle_FormFieldStyleId",
                table: "OneClappForm");

            migrationBuilder.DropForeignKey(
                name: "FK_OneClappForm_OneClappFormStyle_FormStyleId",
                table: "OneClappForm");

            migrationBuilder.DropIndex(
                name: "IX_OneClappForm_FormFieldStyleId",
                table: "OneClappForm");

            migrationBuilder.DropIndex(
                name: "IX_OneClappForm_FormStyleId",
                table: "OneClappForm");

            migrationBuilder.DropColumn(
                name: "FormFieldStyleId",
                table: "OneClappForm");

            migrationBuilder.DropColumn(
                name: "FormStyleId",
                table: "OneClappForm");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FormFieldStyleId",
                table: "OneClappForm",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FormStyleId",
                table: "OneClappForm",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormFieldStyleId",
                table: "OneClappForm",
                column: "FormFieldStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormStyleId",
                table: "OneClappForm",
                column: "FormStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappForm_OneClappFormFieldStyle_FormFieldStyleId",
                table: "OneClappForm",
                column: "FormFieldStyleId",
                principalTable: "OneClappFormFieldStyle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappForm_OneClappFormStyle_FormStyleId",
                table: "OneClappForm",
                column: "FormStyleId",
                principalTable: "OneClappFormStyle",
                principalColumn: "Id");
        }
    }
}
