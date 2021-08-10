namespace PaymentSystem.WalletApp.Web.Infrastructure.TagHelpers
{
    using System.Linq;

    using Microsoft.AspNetCore.Razor.TagHelpers;

    public class HasDataTagHelper : TagHelper
    {
        public bool Exists { get; set; }

        public string Class { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (this.Exists)
            {
                output.Content.SetHtmlContent("<i class=\"fas fa-check-circle\"></i>");
            }
            else
            {
                output.Content.SetHtmlContent("<i class=\"far fa-circle\"></i>");
            }

            this.SetClasses(output);
        }

        private void SetClasses(TagHelperOutput output)
        {
            var classAttr = output.Attributes
                .FirstOrDefault(a => a.Name == "class");

            var @class = string.IsNullOrWhiteSpace(classAttr?.Value.ToString())
                ? this.Class
                : $"{classAttr.Value} {this.Class}";

            output.Attributes.SetAttribute("class", @class);
        }
    }
}
