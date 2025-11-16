using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumbotronEventFinder.Migrations
{
    public partial class AlignPurchaseSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");


            migrationBuilder.RenameColumn(
                name: "CCV",
                table: "Purchases",
                newName: "CVV");

            migrationBuilder.AlterColumn<int>(
                name: "CVV",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "CVV",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "CVV",
                table: "Purchases",
                newName: "CCV");

            migrationBuilder.AlterColumn<int>(
                name: "CardNumber",
                table: "Purchases",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
