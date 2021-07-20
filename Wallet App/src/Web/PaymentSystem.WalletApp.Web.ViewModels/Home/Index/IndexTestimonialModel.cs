namespace PaymentSystem.WalletApp.Web.ViewModels.Home.Index
{
    using PaymentSystem.Common.Mapping;

    using PaymentSystem.WalletApp.Data.Models;

    public class IndexTestimonialModel : IMapFrom<Testimonial>
    {
        public string Comment { get; set; }

        public string Name { get; set; }

        public string UseCase { get; set; }
    }
}
