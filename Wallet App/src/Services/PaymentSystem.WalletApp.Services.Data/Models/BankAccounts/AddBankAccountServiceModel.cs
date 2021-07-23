namespace PaymentSystem.WalletApp.Services.Data.Models.BankAccounts
{
    using AutoMapper;
    using PaymentSystem.Common.Mapping;

    using PaymentSystem.WalletApp.Data.Models;

    public class AddBankAccountServiceModel : IMapTo<BankAccount>, IHaveCustomMappings
    {
        public string Country { get; set; }

        public string BankName { get; set; }

        public string AccountName { get; set; }

        public string IBAN { get; set; }

        public string Swift { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AddBankAccountServiceModel, BankAccount>()
                .ForMember(
                    e => e.AccountHolderName,
                    opt => opt.MapFrom(e => e.AccountName));
        }
    }
}
