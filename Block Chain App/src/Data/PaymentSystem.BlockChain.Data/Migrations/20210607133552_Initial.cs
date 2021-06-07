﻿namespace PaymentSystem.BlockChain.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Address = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Height = table.Column<long>(type: "bigint", nullable: false),
                    BlockHeader_Version = table.Column<int>(type: "int", nullable: true),
                    BlockHeader_PreviousHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockHeader_MerkleRoot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockHeader_TimeStamp = table.Column<long>(type: "bigint", nullable: true),
                    BlockHeader_Difficulty = table.Column<int>(type: "int", nullable: true),
                    BlockHeader_Validator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeStamp = table.Column<long>(type: "bigint", nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Fee = table.Column<double>(type: "float", nullable: false),
                    BlockHash = table.Column<string>(type: "nvarchar(450)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Hash);
                    table.ForeignKey(
                        name: "FK_Transactions_Blocks_BlockHash",
                        column: x => x.BlockHash,
                        principalTable: "Blocks",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_Height",
                table: "Blocks",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockHash",
                table: "Transactions",
                column: "BlockHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Blocks");
        }
    }
}
