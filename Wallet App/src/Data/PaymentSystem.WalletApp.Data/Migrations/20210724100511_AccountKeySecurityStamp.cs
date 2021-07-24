namespace PaymentSystem.WalletApp.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AccountKeySecurityStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "AccountsKeys",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "AccountsKeys");
        }
    }
}
