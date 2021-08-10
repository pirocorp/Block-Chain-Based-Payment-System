namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Index
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    using WalletAccount = PaymentSystem.WalletApp.Data.Models.Account;

    public class AccountIndexListingModel : IMapFrom<WalletAccount>
    {
        public string Address { get; set; }

        public double Balance { get; set; }

        public string FriendlyAddress => AddressHelpers.FriendlyAddress(this.Address);
    }
}
