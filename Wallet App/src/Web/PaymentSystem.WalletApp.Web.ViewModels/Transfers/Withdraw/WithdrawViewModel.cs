namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.Withdraw
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts;

    public class WithdrawViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public IEnumerable<WithdrawBankAccountModel> BankAccounts { get; set; }

        public IEnumerable<CoinAccountModel> CoinAccounts { get; set; }

        public IEnumerable<SelectListItem> Banks => this.BankAccounts
            .Select(b => new SelectListItem($"{b.BankName} - {b.IBAN[^8..]}", b.Id));

        public IEnumerable<SelectListItem> Coins => this.CoinAccounts
            .Select(c => new SelectListItem($"{c.Address[^12..]} - {c.Balance:F2}", c.Address));

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, WithdrawViewModel>()
                .ForMember(d => d.CoinAccounts, opt
                    => opt.MapFrom(s => s.Accounts))
                .ForMember(d => d.BankAccounts, opt
                    => opt.MapFrom(s => s.BankAccounts.Where(b => b.IsApproved)));
        }
    }
}
