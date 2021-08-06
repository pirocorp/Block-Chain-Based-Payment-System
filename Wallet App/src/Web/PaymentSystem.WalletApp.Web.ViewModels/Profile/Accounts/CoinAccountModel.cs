namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts
{
    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class CoinAccountModel : IMapFrom<Account>, IHaveCustomMappings
    {
        public string Address { get; set; }

        public double Balance { get; set; }

        public bool HasStoredKey { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<Account, CoinAccountModel>()
                .ForMember(d => d.HasStoredKey, opt
                    => opt.MapFrom(s => s.AccountKey != null));
        }
    }
}
