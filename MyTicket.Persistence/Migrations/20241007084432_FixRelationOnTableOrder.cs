using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTicket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationOnTableOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTickets_UserId",
                table: "OrderTickets");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTickets_UserId",
                table: "OrderTickets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTickets_UserId",
                table: "OrderTickets");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTickets_UserId",
                table: "OrderTickets",
                column: "UserId",
                unique: true);
        }
    }
}
