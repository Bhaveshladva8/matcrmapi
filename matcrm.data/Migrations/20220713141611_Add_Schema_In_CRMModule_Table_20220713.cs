using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    public partial class Add_Schema_In_CRMModule_Table_20220713 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationNotesComment",
                newName: "OrganizationNotesComment",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationNote",
                newName: "OrganizationNote",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationLabel",
                newName: "OrganizationLabel",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationAttachment",
                newName: "OrganizationAttachment",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationActivityMember",
                newName: "OrganizationActivityMember",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "OrganizationActivity",
                newName: "OrganizationActivity",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Organization",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "LeadNote",
                newName: "LeadNote",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "LeadLabel",
                newName: "LeadLabel",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "LeadActivityMember",
                newName: "LeadActivityMember",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "LeadActivity",
                newName: "LeadActivity",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "Lead",
                newName: "Lead",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "LabelCategory",
                newName: "LabelCategory",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "Label",
                newName: "Label",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerPhone",
                newName: "CustomerPhone",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerNotesComment",
                newName: "CustomerNotesComment",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerNote",
                newName: "CustomerNote",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerLabel",
                newName: "CustomerLabel",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerEmail",
                newName: "CustomerEmail",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerAttachment",
                newName: "CustomerAttachment",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerActivityMember",
                newName: "CustomerActivityMember",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "CustomerActivity",
                newName: "CustomerActivity",
                newSchema: "AppCRM");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customer",
                newSchema: "AppCRM");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OrganizationNotesComment",
                schema: "AppCRM",
                newName: "OrganizationNotesComment");

            migrationBuilder.RenameTable(
                name: "OrganizationNote",
                schema: "AppCRM",
                newName: "OrganizationNote");

            migrationBuilder.RenameTable(
                name: "OrganizationLabel",
                schema: "AppCRM",
                newName: "OrganizationLabel");

            migrationBuilder.RenameTable(
                name: "OrganizationAttachment",
                schema: "AppCRM",
                newName: "OrganizationAttachment");

            migrationBuilder.RenameTable(
                name: "OrganizationActivityMember",
                schema: "AppCRM",
                newName: "OrganizationActivityMember");

            migrationBuilder.RenameTable(
                name: "OrganizationActivity",
                schema: "AppCRM",
                newName: "OrganizationActivity");

            migrationBuilder.RenameTable(
                name: "Organization",
                schema: "AppCRM",
                newName: "Organization");

            migrationBuilder.RenameTable(
                name: "LeadNote",
                schema: "AppCRM",
                newName: "LeadNote");

            migrationBuilder.RenameTable(
                name: "LeadLabel",
                schema: "AppCRM",
                newName: "LeadLabel");

            migrationBuilder.RenameTable(
                name: "LeadActivityMember",
                schema: "AppCRM",
                newName: "LeadActivityMember");

            migrationBuilder.RenameTable(
                name: "LeadActivity",
                schema: "AppCRM",
                newName: "LeadActivity");

            migrationBuilder.RenameTable(
                name: "Lead",
                schema: "AppCRM",
                newName: "Lead");

            migrationBuilder.RenameTable(
                name: "LabelCategory",
                schema: "AppCRM",
                newName: "LabelCategory");

            migrationBuilder.RenameTable(
                name: "Label",
                schema: "AppCRM",
                newName: "Label");

            migrationBuilder.RenameTable(
                name: "CustomerPhone",
                schema: "AppCRM",
                newName: "CustomerPhone");

            migrationBuilder.RenameTable(
                name: "CustomerNotesComment",
                schema: "AppCRM",
                newName: "CustomerNotesComment");

            migrationBuilder.RenameTable(
                name: "CustomerNote",
                schema: "AppCRM",
                newName: "CustomerNote");

            migrationBuilder.RenameTable(
                name: "CustomerLabel",
                schema: "AppCRM",
                newName: "CustomerLabel");

            migrationBuilder.RenameTable(
                name: "CustomerEmail",
                schema: "AppCRM",
                newName: "CustomerEmail");

            migrationBuilder.RenameTable(
                name: "CustomerAttachment",
                schema: "AppCRM",
                newName: "CustomerAttachment");

            migrationBuilder.RenameTable(
                name: "CustomerActivityMember",
                schema: "AppCRM",
                newName: "CustomerActivityMember");

            migrationBuilder.RenameTable(
                name: "CustomerActivity",
                schema: "AppCRM",
                newName: "CustomerActivity");

            migrationBuilder.RenameTable(
                name: "Customer",
                schema: "AppCRM",
                newName: "Customer");
        }
    }
}
