namespace PaymentSystem.WalletApp.Web.ViewModels.Activities.Index
{
    using System.Collections.Generic;

    public class ActivitiesIndexViewModel
    {
        public ActivitiesIndexViewModel()
        {
            this.Activities = new List<ActivityListingModel>();
        }

        public IEnumerable<ActivityListingModel> Activities { get; set; }

        public string DateRange { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
