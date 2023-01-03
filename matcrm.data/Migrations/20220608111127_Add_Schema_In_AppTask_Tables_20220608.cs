using Microsoft.EntityFrameworkCore.Migrations;

namespace matcrm.data.Migrations
{
    public partial class Add_Schema_In_AppTask_Tables_20220608 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AppTask");

            migrationBuilder.RenameTable(
                name: "TaskTimeRecord",
                newName: "TaskTimeRecord",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "TaskStatus",
                newName: "TaskStatus",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "TaskComment",
                newName: "TaskComment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "TaskAttachment",
                newName: "TaskAttachment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "TaskActivity",
                newName: "TaskActivity",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "SubTaskTimeRecord",
                newName: "SubTaskTimeRecord",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "SubTaskComment",
                newName: "SubTaskComment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "SubTaskAttachment",
                newName: "SubTaskAttachment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "SubTaskActivity",
                newName: "SubTaskActivity",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "SectionActivity",
                newName: "SectionActivity",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "Section",
                newName: "Section",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappTaskUser",
                newName: "OneClappTaskUser",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappTask",
                newName: "OneClappTask",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappSubTaskUser",
                newName: "OneClappSubTaskUser",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappSubTask",
                newName: "OneClappSubTask",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappChildTaskUser",
                newName: "OneClappChildTaskUser",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "OneClappChildTask",
                newName: "OneClappChildTask",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "ChildTaskTimeRecord",
                newName: "ChildTaskTimeRecord",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "ChildTaskComment",
                newName: "ChildTaskComment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "ChildTaskAttachment",
                newName: "ChildTaskAttachment",
                newSchema: "AppTask");

            migrationBuilder.RenameTable(
                name: "ChildTaskActivity",
                newName: "ChildTaskActivity",
                newSchema: "AppTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TaskTimeRecord",
                schema: "AppTask",
                newName: "TaskTimeRecord");

            migrationBuilder.RenameTable(
                name: "TaskStatus",
                schema: "AppTask",
                newName: "TaskStatus");

            migrationBuilder.RenameTable(
                name: "TaskComment",
                schema: "AppTask",
                newName: "TaskComment");

            migrationBuilder.RenameTable(
                name: "TaskAttachment",
                schema: "AppTask",
                newName: "TaskAttachment");

            migrationBuilder.RenameTable(
                name: "TaskActivity",
                schema: "AppTask",
                newName: "TaskActivity");

            migrationBuilder.RenameTable(
                name: "SubTaskTimeRecord",
                schema: "AppTask",
                newName: "SubTaskTimeRecord");

            migrationBuilder.RenameTable(
                name: "SubTaskComment",
                schema: "AppTask",
                newName: "SubTaskComment");

            migrationBuilder.RenameTable(
                name: "SubTaskAttachment",
                schema: "AppTask",
                newName: "SubTaskAttachment");

            migrationBuilder.RenameTable(
                name: "SubTaskActivity",
                schema: "AppTask",
                newName: "SubTaskActivity");

            migrationBuilder.RenameTable(
                name: "SectionActivity",
                schema: "AppTask",
                newName: "SectionActivity");

            migrationBuilder.RenameTable(
                name: "Section",
                schema: "AppTask",
                newName: "Section");

            migrationBuilder.RenameTable(
                name: "OneClappTaskUser",
                schema: "AppTask",
                newName: "OneClappTaskUser");

            migrationBuilder.RenameTable(
                name: "OneClappTask",
                schema: "AppTask",
                newName: "OneClappTask");

            migrationBuilder.RenameTable(
                name: "OneClappSubTaskUser",
                schema: "AppTask",
                newName: "OneClappSubTaskUser");

            migrationBuilder.RenameTable(
                name: "OneClappSubTask",
                schema: "AppTask",
                newName: "OneClappSubTask");

            migrationBuilder.RenameTable(
                name: "OneClappChildTaskUser",
                schema: "AppTask",
                newName: "OneClappChildTaskUser");

            migrationBuilder.RenameTable(
                name: "OneClappChildTask",
                schema: "AppTask",
                newName: "OneClappChildTask");

            migrationBuilder.RenameTable(
                name: "ChildTaskTimeRecord",
                schema: "AppTask",
                newName: "ChildTaskTimeRecord");

            migrationBuilder.RenameTable(
                name: "ChildTaskComment",
                schema: "AppTask",
                newName: "ChildTaskComment");

            migrationBuilder.RenameTable(
                name: "ChildTaskAttachment",
                schema: "AppTask",
                newName: "ChildTaskAttachment");

            migrationBuilder.RenameTable(
                name: "ChildTaskActivity",
                schema: "AppTask",
                newName: "ChildTaskActivity");
        }
    }
}
