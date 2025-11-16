using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumbotronEventFinder.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToPurchaseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CCV",
                table: "Purchase",
                newName: "CVV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CVV",
                table: "Purchase",
                newName: "CCV");
        }
    }
}
