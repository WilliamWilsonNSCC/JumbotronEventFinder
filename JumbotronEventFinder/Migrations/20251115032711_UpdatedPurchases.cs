using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumbotronEventFinder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentInfo",
                table: "Purchase",
                newName: "CardNumber");

            migrationBuilder.AddColumn<int>(
                name: "CCV",
                table: "Purchase",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCV",
                table: "Purchase");

            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "Purchase",
                newName: "PaymentInfo");
        }
    }
}
