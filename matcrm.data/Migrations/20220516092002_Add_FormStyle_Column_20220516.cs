using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_FormStyle_Column_20220516 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<object>(
                name: "FormStyle",
                table: "OneClappForm",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<object>(
                name: "HeaderStyle",
                table: "OneClappForm",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<object>(
                name: "LayoutStyle",
                table: "OneClappForm",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormStyle",
                table: "OneClappForm");

            migrationBuilder.DropColumn(
                name: "HeaderStyle",
                table: "OneClappForm");

            migrationBuilder.DropColumn(
                name: "LayoutStyle",
                table: "OneClappForm");
        }
    }
}
