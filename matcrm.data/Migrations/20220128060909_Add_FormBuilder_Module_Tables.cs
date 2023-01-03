using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_FormBuilder_Module_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BorderStyle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorderStyle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoxShadow",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Horizontal = table.Column<long>(type: "bigint", nullable: true),
                    Vertical = table.Column<long>(type: "bigint", nullable: true),
                    Blur = table.Column<long>(type: "bigint", nullable: true),
                    Spread = table.Column<long>(type: "bigint", nullable: true),
                    Position = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxShadow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormAction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormHeader",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    HeaderImage = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CustomHeaderImage = table.Column<string>(type: "varchar(1000)", nullable: true),
                    ImageLink = table.Column<string>(type: "varchar(1000)", nullable: true),
                    ImageSize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormLayoutBackground",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BackgroundImage = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CustomBackgroundImage = table.Column<string>(type: "varchar(1000)", nullable: true),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormLayoutBackground", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "varchar(200)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Typography",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FontFamily = table.Column<string>(type: "varchar(1000)", nullable: true),
                    TextColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    TextStyle = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FontStyle = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FontWeight = table.Column<long>(type: "bigint", nullable: true),
                    FontSize = table.Column<long>(type: "bigint", nullable: true),
                    LineHeight = table.Column<long>(type: "bigint", nullable: true),
                    LetterSpacing = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Typography", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Border",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BorderStyleId = table.Column<long>(type: "bigint", nullable: true),
                    BorderBottom = table.Column<long>(type: "bigint", nullable: true),
                    BorderTop = table.Column<long>(type: "bigint", nullable: true),
                    BorderLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRight = table.Column<long>(type: "bigint", nullable: true),
                    Color = table.Column<string>(type: "varchar(500)", nullable: true),
                    BorderRadiusTopLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusBottomLeft = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusBottomRight = table.Column<long>(type: "bigint", nullable: true),
                    BorderRadiusTopRight = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Border", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Border_BorderStyle_BorderStyleId",
                        column: x => x.BorderStyleId,
                        principalTable: "BorderStyle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormLayout",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextDirectionRTL = table.Column<bool>(type: "boolean", nullable: false),
                    LayoutBackgroundId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormLayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappFormLayout_OneClappFormLayoutBackground_LayoutBackg~",
                        column: x => x.LayoutBackgroundId,
                        principalTable: "OneClappFormLayoutBackground",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappRequestForm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OneClappFormId = table.Column<long>(type: "bigint", nullable: true),
                    OneClappFormStatusId = table.Column<long>(type: "bigint", nullable: true),
                    IsVerify = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappRequestForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappRequestForm_OneClappFormStatus_OneClappFormStatusId",
                        column: x => x.OneClappFormStatusId,
                        principalTable: "OneClappFormStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormFieldStyle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarginTop = table.Column<long>(type: "bigint", nullable: true),
                    MarginBottom = table.Column<long>(type: "bigint", nullable: true),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Width = table.Column<string>(type: "varchar(1000)", nullable: true),
                    BorderId = table.Column<long>(type: "bigint", nullable: true),
                    TypographyId = table.Column<long>(type: "bigint", nullable: true),
                    BoxShadowId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormFieldStyle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldStyle_Border_BorderId",
                        column: x => x.BorderId,
                        principalTable: "Border",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldStyle_BoxShadow_BoxShadowId",
                        column: x => x.BoxShadowId,
                        principalTable: "BoxShadow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldStyle_Typography_TypographyId",
                        column: x => x.TypographyId,
                        principalTable: "Typography",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormStyle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Width = table.Column<long>(type: "bigint", nullable: true),
                    BackgroundColor = table.Column<string>(type: "varchar(1000)", nullable: true),
                    BorderId = table.Column<long>(type: "bigint", nullable: true),
                    BoxShadowId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormStyle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappFormStyle_Border_BorderId",
                        column: x => x.BorderId,
                        principalTable: "Border",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormStyle_BoxShadow_BoxShadowId",
                        column: x => x.BoxShadowId,
                        principalTable: "BoxShadow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappForm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    FormGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    FormKey = table.Column<string>(type: "text", nullable: true),
                    FormTypeId = table.Column<long>(type: "bigint", nullable: true),
                    FormActionId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ButtonText = table.Column<string>(type: "varchar(1000)", nullable: true),
                    ButtonCssClass = table.Column<string>(type: "varchar(1000)", nullable: true),
                    IsUsePlaceHolder = table.Column<bool>(type: "boolean", nullable: false),
                    IsUseCssClass = table.Column<bool>(type: "boolean", nullable: false),
                    RedirectUrl = table.Column<string>(type: "text", nullable: true),
                    EmbededUrl = table.Column<string>(type: "text", nullable: true),
                    EmbededCode = table.Column<string>(type: "text", nullable: true),
                    FormStyleId = table.Column<long>(type: "bigint", nullable: true),
                    FormFieldStyleId = table.Column<long>(type: "bigint", nullable: true),
                    FormHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    FormLayoutId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormAction_FormActionId",
                        column: x => x.FormActionId,
                        principalTable: "OneClappFormAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormFieldStyle_FormFieldStyleId",
                        column: x => x.FormFieldStyleId,
                        principalTable: "OneClappFormFieldStyle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormHeader_FormHeaderId",
                        column: x => x.FormHeaderId,
                        principalTable: "OneClappFormHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormLayout_FormLayoutId",
                        column: x => x.FormLayoutId,
                        principalTable: "OneClappFormLayout",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormStyle_FormStyleId",
                        column: x => x.FormStyleId,
                        principalTable: "OneClappFormStyle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_OneClappFormType_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "OneClappFormType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappForm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormField",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OneClappFormId = table.Column<long>(type: "bigint", nullable: true),
                    CustomFieldId = table.Column<long>(type: "bigint", nullable: true),
                    CustomTableColumnId = table.Column<long>(type: "bigint", nullable: true),
                    CustomModuleId = table.Column<long>(type: "bigint", nullable: true),
                    LabelName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    PlaceHolder = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CssClassName = table.Column<string>(type: "varchar(1000)", nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappFormField_CustomField_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormField_CustomModule_CustomModuleId",
                        column: x => x.CustomModuleId,
                        principalTable: "CustomModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormField_CustomTableColumn_CustomTableColumnId",
                        column: x => x.CustomTableColumnId,
                        principalTable: "CustomTableColumn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormField_OneClappForm_OneClappFormId",
                        column: x => x.OneClappFormId,
                        principalTable: "OneClappForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OneClappFormFieldValue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OneClappFormId = table.Column<long>(type: "bigint", nullable: true),
                    OneClappFormFieldId = table.Column<long>(type: "bigint", nullable: true),
                    CustomFieldId = table.Column<long>(type: "bigint", nullable: true),
                    CustomTableColumnId = table.Column<long>(type: "bigint", nullable: true),
                    OneClappRequestFormId = table.Column<long>(type: "bigint", nullable: true),
                    OptionId = table.Column<long>(type: "bigint", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneClappFormFieldValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_CustomControlOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "CustomControlOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_CustomField_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_CustomTableColumn_CustomTableColumnId",
                        column: x => x.CustomTableColumnId,
                        principalTable: "CustomTableColumn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_OneClappForm_OneClappFormId",
                        column: x => x.OneClappFormId,
                        principalTable: "OneClappForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_OneClappFormField_OneClappFormFieldId",
                        column: x => x.OneClappFormFieldId,
                        principalTable: "OneClappFormField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OneClappFormFieldValue_OneClappRequestForm_OneClappRequestF~",
                        column: x => x.OneClappRequestFormId,
                        principalTable: "OneClappRequestForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Border_BorderStyleId",
                table: "Border",
                column: "BorderStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormActionId",
                table: "OneClappForm",
                column: "FormActionId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormFieldStyleId",
                table: "OneClappForm",
                column: "FormFieldStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormHeaderId",
                table: "OneClappForm",
                column: "FormHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormLayoutId",
                table: "OneClappForm",
                column: "FormLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormStyleId",
                table: "OneClappForm",
                column: "FormStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_FormTypeId",
                table: "OneClappForm",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappForm_TenantId",
                table: "OneClappForm",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormField_CustomFieldId",
                table: "OneClappFormField",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormField_CustomModuleId",
                table: "OneClappFormField",
                column: "CustomModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormField_CustomTableColumnId",
                table: "OneClappFormField",
                column: "CustomTableColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormField_OneClappFormId",
                table: "OneClappFormField",
                column: "OneClappFormId");

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

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_CustomFieldId",
                table: "OneClappFormFieldValue",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_CustomTableColumnId",
                table: "OneClappFormFieldValue",
                column: "CustomTableColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_OneClappFormFieldId",
                table: "OneClappFormFieldValue",
                column: "OneClappFormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_OneClappFormId",
                table: "OneClappFormFieldValue",
                column: "OneClappFormId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_OneClappRequestFormId",
                table: "OneClappFormFieldValue",
                column: "OneClappRequestFormId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormFieldValue_OptionId",
                table: "OneClappFormFieldValue",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormLayout_LayoutBackgroundId",
                table: "OneClappFormLayout",
                column: "LayoutBackgroundId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormStyle_BorderId",
                table: "OneClappFormStyle",
                column: "BorderId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappFormStyle_BoxShadowId",
                table: "OneClappFormStyle",
                column: "BoxShadowId");

            migrationBuilder.CreateIndex(
                name: "IX_OneClappRequestForm_OneClappFormStatusId",
                table: "OneClappRequestForm",
                column: "OneClappFormStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneClappFormFieldValue");

            migrationBuilder.DropTable(
                name: "OneClappFormField");

            migrationBuilder.DropTable(
                name: "OneClappRequestForm");

            migrationBuilder.DropTable(
                name: "OneClappForm");

            migrationBuilder.DropTable(
                name: "OneClappFormStatus");

            migrationBuilder.DropTable(
                name: "OneClappFormAction");

            migrationBuilder.DropTable(
                name: "OneClappFormFieldStyle");

            migrationBuilder.DropTable(
                name: "OneClappFormHeader");

            migrationBuilder.DropTable(
                name: "OneClappFormLayout");

            migrationBuilder.DropTable(
                name: "OneClappFormStyle");

            migrationBuilder.DropTable(
                name: "OneClappFormType");

            migrationBuilder.DropTable(
                name: "Typography");

            migrationBuilder.DropTable(
                name: "OneClappFormLayoutBackground");

            migrationBuilder.DropTable(
                name: "Border");

            migrationBuilder.DropTable(
                name: "BoxShadow");

            migrationBuilder.DropTable(
                name: "BorderStyle");
        }
    }
}
