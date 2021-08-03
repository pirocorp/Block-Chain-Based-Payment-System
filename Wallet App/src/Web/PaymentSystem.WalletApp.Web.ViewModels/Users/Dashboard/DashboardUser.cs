namespace PaymentSystem.WalletApp.Web.ViewModels.Users.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    public class DashboardUser : ProfileLayoutUserModel, IHaveCustomMappings
    {
        public DashboardUser()
        {
            this.Activities = new List<ActivityListingModel>();
        }

        public bool HasPhoneAdded { get; set; }

        public bool HasEmailAdded { get; set; }

        public bool HasCardAdded { get; set; }

        public bool HasBankAccountAdded { get; set; }

        public IEnumerable<ActivityListingModel> Activities { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, DashboardUser>()
                .ForMember(d => d.HasPhoneAdded, opt
                    => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.PhoneNumber)))
                .ForMember(d => d.HasEmailAdded, opt
                    => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.Email)))
                .ForMember(d => d.HasCardAdded, opt
                    => opt.MapFrom(s => s.CreditCards.Any()))
                .ForMember(d => d.HasBankAccountAdded, opt
                    => opt.MapFrom(s => s.BankAccounts.Any()))
                .ForMember(d => d.TotalBalance, opt
                    => opt.MapFrom(s => s.Accounts.Sum(a => a.Balance)))
                .ForMember(d => d.Activities, opt 
                    => opt.MapFrom(s => s.Activities.OrderByDescending(a => a.TimeStamp).Take(WebConstants.DefaultActivitiesResultPageSize)));
        }
    }
}
