namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Profile
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    using PaymentSystem.WalletApp.Services.Data.Models.Accounts;

    public class EditAccountDetailsModel : IMapFrom<Account>, IMapTo<EditAccountServiceModel>
    {
        public string Address { get; set; }

        public string Name { get; set; }
    }
}
