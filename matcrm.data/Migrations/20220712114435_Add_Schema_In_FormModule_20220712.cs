using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Schema_In_FormModule_20220712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappRequestForm",
                newName: "OneClappRequestForm",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormType",
                newName: "OneClappFormType",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormStatus",
                newName: "OneClappFormStatus",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormLayoutBackground",
                newName: "OneClappFormLayoutBackground",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormLayout",
                newName: "OneClappFormLayout",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormHeader",
                newName: "OneClappFormHeader",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormFieldValue",
                newName: "OneClappFormFieldValue",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormField",
                newName: "OneClappFormField",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormAction",
                newName: "OneClappFormAction",
                newSchema: "AppForm");

            migrationBuilder.RenameTable(
                name: "OneClappForm",
                newName: "OneClappForm",
                newSchema: "AppForm");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OneClappRequestForm",
                schema: "AppForm",
                newName: "OneClappRequestForm");

            migrationBuilder.RenameTable(
                name: "OneClappFormType",
                schema: "AppForm",
                newName: "OneClappFormType");

            migrationBuilder.RenameTable(
                name: "OneClappFormStatus",
                schema: "AppForm",
                newName: "OneClappFormStatus");

            migrationBuilder.RenameTable(
                name: "OneClappFormLayoutBackground",
                schema: "AppForm",
                newName: "OneClappFormLayoutBackground");

            migrationBuilder.RenameTable(
                name: "OneClappFormLayout",
                schema: "AppForm",
                newName: "OneClappFormLayout");

            migrationBuilder.RenameTable(
                name: "OneClappFormHeader",
                schema: "AppForm",
                newName: "OneClappFormHeader");

            migrationBuilder.RenameTable(
                name: "OneClappFormFieldValue",
                schema: "AppForm",
                newName: "OneClappFormFieldValue");

            migrationBuilder.RenameTable(
                name: "OneClappFormField",
                schema: "AppForm",
                newName: "OneClappFormField");

            migrationBuilder.RenameTable(
                name: "OneClappFormAction",
                schema: "AppForm",
                newName: "OneClappFormAction");

            migrationBuilder.RenameTable(
                name: "OneClappForm",
                schema: "AppForm",
                newName: "OneClappForm");
        }
    }
}
