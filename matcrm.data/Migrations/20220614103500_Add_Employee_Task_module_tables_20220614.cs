using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace matcrm.data.Migrations
{
    public partial class Add_Employee_Task_module_tables_20220614 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeProject",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(1500)", nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProject_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTaskTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    SubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskTimeRecord_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskStatus",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "varchar(200)", nullable: true),
                    IsFinalize = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskStatus_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: true),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTask_EmployeeProject_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTask_EmployeeTaskStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTask_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProjectActivity",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Activity = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProjectActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProjectActivity_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProjectActivity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeClappTimeRecordId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTask_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTask_EmployeeTaskStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskActivity",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Activity = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskActivity_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskActivity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskAttachment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskAttachment_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskAttachment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskComment_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskTimeRecord_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskTimeRecord_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTaskUser",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskUser_EmployeeTask_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTask",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeClappTimeRecordId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeSubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTask_EmployeeSubTask_EmployeeSubTaskId",
                        column: x => x.EmployeeSubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTask_EmployeeTaskStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeTaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTaskActivity",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeSubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Activity = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTaskActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskActivity_EmployeeSubTask_EmployeeSubTaskId",
                        column: x => x.EmployeeSubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskActivity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTaskAttachment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeSubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTaskAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskAttachment_EmployeeSubTask_EmployeeSubTaskId",
                        column: x => x.EmployeeSubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskAttachment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeSubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskComment_EmployeeSubTask_EmployeeSubTaskId",
                        column: x => x.EmployeeSubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubTaskUser",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeSubTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubTaskUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSubTaskUser_EmployeeSubTask_EmployeeSubTaskId",
                        column: x => x.EmployeeSubTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeSubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTaskActivity",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Activity = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTaskActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskActivity_EmployeeChildTask_EmployeeChildTa~",
                        column: x => x.EmployeeChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskActivity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTaskAttachment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTaskAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskAttachment_EmployeeChildTask_EmployeeChild~",
                        column: x => x.EmployeeChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskAttachment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTaskComment",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "varchar(1000)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskComment_EmployeeChildTask_EmployeeChildTas~",
                        column: x => x.EmployeeChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTaskTimeRecord",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTaskTimeRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskTimeRecord_EmployeeChildTask_EmployeeChild~",
                        column: x => x.EmployeeChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskTimeRecord_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChildTaskUser",
                schema: "AppTask",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeChildTaskId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChildTaskUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeChildTaskUser_EmployeeChildTask_EmployeeChildTaskId",
                        column: x => x.EmployeeChildTaskId,
                        principalSchema: "AppTask",
                        principalTable: "EmployeeChildTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTask_EmployeeSubTaskId",
                schema: "AppTask",
                table: "EmployeeChildTask",
                column: "EmployeeSubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTask_StatusId",
                schema: "AppTask",
                table: "EmployeeChildTask",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskActivity_EmployeeChildTaskId",
                schema: "AppTask",
                table: "EmployeeChildTaskActivity",
                column: "EmployeeChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskActivity_UserId",
                schema: "AppTask",
                table: "EmployeeChildTaskActivity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskAttachment_EmployeeChildTaskId",
                schema: "AppTask",
                table: "EmployeeChildTaskAttachment",
                column: "EmployeeChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskAttachment_UserId",
                schema: "AppTask",
                table: "EmployeeChildTaskAttachment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskComment_EmployeeChildTaskId",
                schema: "AppTask",
                table: "EmployeeChildTaskComment",
                column: "EmployeeChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskComment_UserId",
                schema: "AppTask",
                table: "EmployeeChildTaskComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskTimeRecord_EmployeeChildTaskId",
                schema: "AppTask",
                table: "EmployeeChildTaskTimeRecord",
                column: "EmployeeChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskTimeRecord_UserId",
                schema: "AppTask",
                table: "EmployeeChildTaskTimeRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeChildTaskUser_EmployeeChildTaskId",
                schema: "AppTask",
                table: "EmployeeChildTaskUser",
                column: "EmployeeChildTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProject_TenantId",
                schema: "AppTask",
                table: "EmployeeProject",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectActivity_ProjectId",
                schema: "AppTask",
                table: "EmployeeProjectActivity",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectActivity_UserId",
                schema: "AppTask",
                table: "EmployeeProjectActivity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTask_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeSubTask",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTask_StatusId",
                schema: "AppTask",
                table: "EmployeeSubTask",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskActivity_EmployeeSubTaskId",
                schema: "AppTask",
                table: "EmployeeSubTaskActivity",
                column: "EmployeeSubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskActivity_UserId",
                schema: "AppTask",
                table: "EmployeeSubTaskActivity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskAttachment_EmployeeSubTaskId",
                schema: "AppTask",
                table: "EmployeeSubTaskAttachment",
                column: "EmployeeSubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskAttachment_UserId",
                schema: "AppTask",
                table: "EmployeeSubTaskAttachment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskComment_EmployeeSubTaskId",
                schema: "AppTask",
                table: "EmployeeSubTaskComment",
                column: "EmployeeSubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskComment_UserId",
                schema: "AppTask",
                table: "EmployeeSubTaskComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskTimeRecord_UserId",
                schema: "AppTask",
                table: "EmployeeSubTaskTimeRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubTaskUser_EmployeeSubTaskId",
                schema: "AppTask",
                table: "EmployeeSubTaskUser",
                column: "EmployeeSubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_ProjectId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_StatusId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_TenantId",
                schema: "AppTask",
                table: "EmployeeTask",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskActivity_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeTaskActivity",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskActivity_UserId",
                schema: "AppTask",
                table: "EmployeeTaskActivity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskAttachment_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeTaskAttachment",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskAttachment_UserId",
                schema: "AppTask",
                table: "EmployeeTaskAttachment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskComment_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeTaskComment",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskComment_UserId",
                schema: "AppTask",
                table: "EmployeeTaskComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskStatus_TenantId",
                schema: "AppTask",
                table: "EmployeeTaskStatus",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskTimeRecord_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskTimeRecord_UserId",
                schema: "AppTask",
                table: "EmployeeTaskTimeRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskUser_EmployeeTaskId",
                schema: "AppTask",
                table: "EmployeeTaskUser",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskUser_UserId",
                schema: "AppTask",
                table: "EmployeeTaskUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeChildTaskActivity",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeChildTaskAttachment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeChildTaskComment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeChildTaskTimeRecord",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeChildTaskUser",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeProjectActivity",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeSubTaskActivity",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeSubTaskAttachment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeSubTaskComment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeSubTaskTimeRecord",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeSubTaskUser",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskActivity",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskAttachment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskComment",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskTimeRecord",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskUser",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeChildTask",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "EmployeeSubTask",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTask",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeProject",
                schema: "AppTask");

            migrationBuilder.DropTable(
                name: "EmployeeTaskStatus",
                schema: "AppTask");
        }
    }
}
