namespace PaymentSystem.WalletApp.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CreditCardColumnsChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVV",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "CardHolderName",
                table: "CreditCards");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "CreditCards",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "CreditCards",
                newName: "CardData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "CreditCards",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "CardData",
                table: "CreditCards",
                newName: "CardNumber");

            migrationBuilder.AddColumn<string>(
                name: "CVV",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardHolderName",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
