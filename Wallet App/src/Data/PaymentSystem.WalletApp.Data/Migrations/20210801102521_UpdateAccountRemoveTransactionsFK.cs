using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentSystem.WalletApp.Data.Migrations
{
    public partial class UpdateAccountRemoveTransactionsFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_Recipient",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_Sender",
                table: "Transactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_Recipient",
                table: "Transactions",
                column: "Recipient",
                principalTable: "Accounts",
                principalColumn: "Address",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_Sender",
                table: "Transactions",
                column: "Sender",
                principalTable: "Accounts",
                principalColumn: "Address",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
