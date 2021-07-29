using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentSystem.WalletApp.Data.Migrations
{
    public partial class ActivityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionHash",
                table: "Activities",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionHash",
                table: "Activities");
        }
    }
}
