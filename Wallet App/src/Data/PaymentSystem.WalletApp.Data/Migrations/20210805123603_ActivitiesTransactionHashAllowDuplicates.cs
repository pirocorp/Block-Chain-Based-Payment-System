using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentSystem.WalletApp.Data.Migrations
{
    public partial class ActivitiesTransactionHashAllowDuplicates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activities_TransactionHash",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TransactionHash",
                table: "Activities",
                column: "TransactionHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activities_TransactionHash",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TransactionHash",
                table: "Activities",
                column: "TransactionHash",
                unique: true);
        }
    }
}
