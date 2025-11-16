using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumbotronEventFinder.Migrations
{
    /// <inheritdoc />
    public partial class FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Show_ShowId",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchase");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_ShowId",
                table: "Purchase",
                newName: "IX_Purchase_ShowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Show_ShowId",
                table: "Purchase",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "ShowId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Show_ShowId",
                table: "Purchase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase");

            migrationBuilder.RenameTable(
                name: "Purchase",
                newName: "Purchases");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_ShowId",
                table: "Purchases",
                newName: "IX_Purchases_ShowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Show_ShowId",
                table: "Purchases",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "ShowId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
