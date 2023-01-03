using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_Priority_Column_InTo_FormField_Table_20220520 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "OneClappFormField",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "OneClappFormField");
        }
    }
}
