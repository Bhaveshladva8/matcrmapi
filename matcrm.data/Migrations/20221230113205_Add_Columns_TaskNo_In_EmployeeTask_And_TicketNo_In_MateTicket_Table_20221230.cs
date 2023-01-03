using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace matcrm.data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsTaskNoInEmployeeTaskAndTicketNoInMateTicketTable20221230 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketNo",
                schema: "AppTask",
                table: "MateTicket",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskNo",
                schema: "AppTask",
                table: "EmployeeTask",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketNo",
                schema: "AppTask",
                table: "MateTicket");

            migrationBuilder.DropColumn(
                name: "TaskNo",
                schema: "AppTask",
                table: "EmployeeTask");
        }
    }
}
