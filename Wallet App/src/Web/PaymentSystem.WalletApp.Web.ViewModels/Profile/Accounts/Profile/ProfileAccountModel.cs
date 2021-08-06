namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileAccountModel : IMapFrom<Account>
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public double Balance { get; set; }
    }
}
