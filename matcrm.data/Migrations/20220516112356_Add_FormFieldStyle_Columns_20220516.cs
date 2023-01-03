using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_FormFieldStyle_Columns_20220516 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<object>(
                name: "FormFieldStyle",
                table: "OneClappFormField",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<object>(
                name: "TypographyStyle",
                table: "OneClappFormField",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormFieldStyle",
                table: "OneClappFormField");

            migrationBuilder.DropColumn(
                name: "TypographyStyle",
                table: "OneClappFormField");
        }
    }
}
