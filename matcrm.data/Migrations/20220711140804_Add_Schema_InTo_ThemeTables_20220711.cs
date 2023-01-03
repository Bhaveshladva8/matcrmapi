using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Schema_InTo_ThemeTables_20220711 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppTheme");

            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeScheme",
                newName: "OneClappLatestThemeScheme",
                newSchema: "AppTheme");

            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeLayout",
                newName: "OneClappLatestThemeLayout",
                newSchema: "AppTheme");

            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeConfig",
                newName: "OneClappLatestThemeConfig",
                newSchema: "AppTheme");

            migrationBuilder.RenameTable(
                name: "OneClappLatestTheme",
                newName: "OneClappLatestTheme",
                newSchema: "AppTheme");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeScheme",
                schema: "AppTheme",
                newName: "OneClappLatestThemeScheme");

            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeLayout",
                schema: "AppTheme",
                newName: "OneClappLatestThemeLayout");

            migrationBuilder.RenameTable(
                name: "OneClappLatestThemeConfig",
                schema: "AppTheme",
                newName: "OneClappLatestThemeConfig");

            migrationBuilder.RenameTable(
                name: "OneClappLatestTheme",
                schema: "AppTheme",
                newName: "OneClappLatestTheme");
        }
    }
}
