namespace PaymentSystem.BlockChain.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class TableAccountPublicKeyProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "Accounts");
        }
    }
}
