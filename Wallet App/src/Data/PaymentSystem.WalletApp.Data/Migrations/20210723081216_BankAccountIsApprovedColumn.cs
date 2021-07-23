using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentSystem.WalletApp.Data.Migrations
{
    public partial class BankAccountIsApprovedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "BankAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "BankAccounts");
        }
    }
}
