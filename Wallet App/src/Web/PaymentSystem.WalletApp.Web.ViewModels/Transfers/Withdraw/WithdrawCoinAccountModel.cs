namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.Withdraw
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class WithdrawCoinAccountModel : IMapFrom<Account>
    {
        public string Address { get; set; }

        public double Balance { get; set; }
    }
}
