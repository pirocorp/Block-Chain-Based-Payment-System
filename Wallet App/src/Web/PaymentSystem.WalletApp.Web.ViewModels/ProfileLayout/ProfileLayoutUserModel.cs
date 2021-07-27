namespace PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout
{
    using System.Linq;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class ProfileLayoutUserModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        public double TotalBalance { get; set; }

        public string ProfilePictureAddress { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ApplicationUser, ProfileLayoutUserModel>()
                .ForMember(d => d.TotalBalance, opt
                    => opt.MapFrom(s => s.Accounts.Sum(a => a.Balance)));
        }
    }
}
