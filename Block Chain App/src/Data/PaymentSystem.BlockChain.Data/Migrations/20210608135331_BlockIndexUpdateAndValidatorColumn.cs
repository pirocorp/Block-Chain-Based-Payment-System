namespace PaymentSystem.BlockChain.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class BlockIndexUpdateAndValidatorColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blocks_Height",
                table: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "Validator",
                table: "Blocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_Height",
                table: "Blocks",
                column: "Height",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blocks_Height",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "Validator",
                table: "Blocks");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_Height",
                table: "Blocks",
                column: "Height");
        }
    }
}
