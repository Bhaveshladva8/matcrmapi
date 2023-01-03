using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Remove_ForeignKey_From_FormStyle_And_FormFieldStyle_Table_20220712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OneClappFormFieldStyle_Border_BorderId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropForeignKey(
                name: "FK_OneClappFormFieldStyle_BoxShadow_BoxShadowId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropForeignKey(
                name: "FK_OneClappFormFieldStyle_Typography_TypographyId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropForeignKey(
                name: "FK_OneClappFormStyle_Border_BorderId",
                table: "OneClappFormStyle");

            migrationBuilder.DropForeignKey(
                name: "FK_OneClappFormStyle_BoxShadow_BoxShadowId",
                table: "OneClappFormStyle");

            migrationBuilder.DropIndex(
                name: "IX_OneClappFormStyle_BorderId",
                table: "OneClappFormStyle");

            migrationBuilder.DropIndex(
                name: "IX_OneClappFormStyle_BoxShadowId",
                table: "OneClappFormStyle");

            migrationBuilder.DropIndex(
                name: "IX_OneClappFormFieldStyle_BorderId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropIndex(
                name: "IX_OneClappFormFieldStyle_BoxShadowId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropIndex(
                name: "IX_OneClappFormFieldStyle_TypographyId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropColumn(
                name: "BorderId",
                table: "OneClappFormStyle");

            migrationBuilder.DropColumn(
                name: "BoxShadowId",
                table: "OneClappFormStyle");

            migrationBuilder.DropColumn(
                name: "BorderId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropColumn(
                name: "BoxShadowId",
                table: "OneClappFormFieldStyle");

            migrationBuilder.DropColumn(
                name: "TypographyId",
                table: "OneClappFormFieldStyle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BorderId",
                table: "OneClappFormStyle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BoxShadowId",
                table: "OneClappFormStyle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BorderId",
                table: "OneClappFormFieldStyle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BoxShadowId",
                table: "OneClappFormFieldStyle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TypographyId",
                table: "OneClappFormFieldStyle",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormStyle_BorderId",
                table: "OneClappFormStyle",
                column: "BorderId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormStyle_BoxShadowId",
                table: "OneClappFormStyle",
                column: "BoxShadowId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldStyle_BorderId",
                table: "OneClappFormFieldStyle",
                column: "BorderId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldStyle_BoxShadowId",
                table: "OneClappFormFieldStyle",
                column: "BoxShadowId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldStyle_TypographyId",
                table: "OneClappFormFieldStyle",
                column: "TypographyId");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappFormFieldStyle_Border_BorderId",
                table: "OneClappFormFieldStyle",
                column: "BorderId",
                principalTable: "Border",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappFormFieldStyle_BoxShadow_BoxShadowId",
                table: "OneClappFormFieldStyle",
                column: "BoxShadowId",
                principalTable: "BoxShadow",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappFormFieldStyle_Typography_TypographyId",
                table: "OneClappFormFieldStyle",
                column: "TypographyId",
                principalTable: "Typography",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappFormStyle_Border_BorderId",
                table: "OneClappFormStyle",
                column: "BorderId",
                principalTable: "Border",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OneClappFormStyle_BoxShadow_BoxShadowId",
                table: "OneClappFormStyle",
                column: "BoxShadowId",
                principalTable: "BoxShadow",
                principalColumn: "Id");
        }
    }
}
