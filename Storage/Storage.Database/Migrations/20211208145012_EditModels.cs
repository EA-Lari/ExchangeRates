using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Database.Migrations
{
    public partial class EditModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates");

            migrationBuilder.DropColumn(
                name: "CurrancyId",
                table: "CurrencyRates");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                table: "CurrencyRates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                table: "CurrencyRates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CurrancyId",
                table: "CurrencyRates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
