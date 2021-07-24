namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Profile
{
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    public class NewAccountDetailsViewModel : ProfileLayoutUserModel
    {
        public AccountCreationResponse NewAccountDetails { get; set; }
    }
}
