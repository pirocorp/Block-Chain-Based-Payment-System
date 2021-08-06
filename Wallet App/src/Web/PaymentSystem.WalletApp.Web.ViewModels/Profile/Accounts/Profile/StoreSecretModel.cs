namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models.AccountsKeys;

    public class StoreSecretModel : IMapTo<StoreAccountKeyServiceModel>
    {
        public string Address { get; set; }

        public string Secret { get; set; }
    }
}
