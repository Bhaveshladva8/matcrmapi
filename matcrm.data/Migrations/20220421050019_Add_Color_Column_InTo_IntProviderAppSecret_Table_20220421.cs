using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_Color_Column_InTo_IntProviderAppSecret_Table_20220421 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "IntProviderAppSecret",
                type: "varchar(500)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "IntProviderAppSecret");
        }
    }
}
