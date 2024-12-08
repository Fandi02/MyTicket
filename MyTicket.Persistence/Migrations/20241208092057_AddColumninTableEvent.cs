using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTicket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumninTableEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableTicket",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTicket",
                table: "Events");
        }
    }
}
