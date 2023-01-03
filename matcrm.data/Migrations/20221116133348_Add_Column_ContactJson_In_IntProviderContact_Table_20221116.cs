using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Column_ContactJson_In_IntProviderContact_Table_20221116 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<object>(
                name: "ContactJson",
                schema: "AppCRM",
                table: "IntProviderContact",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactJson",
                schema: "AppCRM",
                table: "IntProviderContact");
        }
    }
}
