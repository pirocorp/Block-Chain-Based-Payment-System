namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts.Profile
{
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.ProfileLayout;

    public class NewAccountDetailsViewModel : ProfileLayoutUserModel
    {
        public AccountCreationResponse NewAccountDetails { get; set; }
    }
}
