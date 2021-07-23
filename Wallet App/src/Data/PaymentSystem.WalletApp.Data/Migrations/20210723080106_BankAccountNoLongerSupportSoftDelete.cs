using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentSystem.WalletApp.Data.Migrations
{
    public partial class BankAccountNoLongerSupportSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_IsDeleted",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BankAccounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "BankAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BankAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_IsDeleted",
                table: "BankAccounts",
                column: "IsDeleted");
        }
    }
}
