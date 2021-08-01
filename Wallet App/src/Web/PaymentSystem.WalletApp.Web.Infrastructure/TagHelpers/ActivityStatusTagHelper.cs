namespace PaymentSystem.WalletApp.Web.Infrastructure.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using PaymentSystem.WalletApp.Data.Models;

    public class ActivityStatusTagHelper : TagHelper
    {
        public ActivityStatus Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            switch (this.Status)
            {
                case ActivityStatus.Pending:
                    output.Attributes.SetAttribute("class", "text-warning");
                    output.Content.SetHtmlContent("<i class=\"fas fa-ellipsis-h\"></i>");
                    break;
                case ActivityStatus.Completed:
                    output.Attributes.SetAttribute("class", "text-success");
                    output.Content.SetHtmlContent("<i class=\"fas fa-check-circle\"></i>");
                    break;
                case ActivityStatus.Canceled:
                    output.Attributes.SetAttribute("class", "text-danger");
                    output.Content.SetHtmlContent("<i class=\"fas fa-times-circle\"></i>");
                    break;
            }
        }
    }
}
