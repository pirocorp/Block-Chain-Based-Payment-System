namespace PaymentSystem.WalletApp.Web.Infrastructure.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    using Microsoft.AspNetCore.Razor.TagHelpers;

    public class PaginationTagHelper : TagHelper
    {
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const int Offset = 2;

        public PaginationTagHelper()
        {
            this.RouteValues = new Dictionary<string, string>();
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        [HtmlAttributeName("active-page-class")]
        public string ActivePageClass { get; set; }

        [HtmlAttributeName("disabled-page-class")]
        public string DisabledPageClass { get; set; }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-page")]
        public string Page { get; set; }

        [HtmlAttributeName("page-item-class")]
        public string PageItemClass { get; set; }

        [HtmlAttributeName("page-link-class")]
        public string PageLinkClass { get; set; }

        [HtmlAttributeName("empty-link-class")]
        public string EmptyLinkClass { get; set; }

        [Range(2, int.MaxValue)]
        public int Boundary { get; set; }

        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = new StringBuilder();

            if (this.TotalPages > 3 + (2 * this.Boundary))
            {
                var maxLowerBoundary = Math.Max(Math.Min(this.CurrentPage + 1, this.Boundary + Offset), this.Boundary);
                var lowerBoundaryLimit = maxLowerBoundary > this.CurrentPage
                    ? maxLowerBoundary
                    : this.Boundary;

                for (var i = 1; i <= lowerBoundaryLimit; i++)
                {
                    this.CreatePaginationLink(i, content);
                }

                if (this.CurrentPage > this.Boundary + Offset)
                {
                    content.AppendLine($"<li class=\"{this.EmptyLinkClass}\">...</li>");

                }

                if (this.CurrentPage > this.Boundary + 1 && this.CurrentPage < this.TotalPages - this.Boundary)
                {
                    for (var i = this.CurrentPage - 1; i <= this.CurrentPage + 1; i++)
                    {
                        this.CreatePaginationLink(i, content);
                    }
                }

                if (this.CurrentPage < this.TotalPages - (this.Boundary + 1))
                {
                    content.AppendLine($"<li class=\"{this.EmptyLinkClass}\">...</li>");
                }

                var upperBoundary = this.Boundary - 1;

                var upperBoundaryLimit = this.CurrentPage >= this.TotalPages - this.Boundary
                    ? Math.Min(this.CurrentPage - 1, this.TotalPages - upperBoundary)
                    : this.TotalPages - upperBoundary;

                for (var i = upperBoundaryLimit; i <= this.TotalPages; i++)
                {
                    this.CreatePaginationLink(i, content);
                }
            }
            else
            {
                for (var i = 1; i <= this.TotalPages; i++)
                {
                    this.CreatePaginationLink(i, content);
                }
            }

            output.Content.SetHtmlContent(content.ToString());

            this.SetPaginatorClasses(output);
        }

        private static string LinkPart(string part)
            => string.IsNullOrWhiteSpace(part) ? string.Empty : $"/{part}";

        private string GetLinkAddress() => $"{LinkPart(this.Area)}{LinkPart(this.Controller)}{LinkPart(this.Action)}{LinkPart(this.Page)}";

        private void CreatePaginationLink(int page, StringBuilder content)
        {
            var activePage = page == this.CurrentPage ? " active" : string.Empty;
            var currentPage = page == this.CurrentPage ? "<span class=\"sr-only\">(current)</span>": string.Empty;

            var routeValues = this.RouteValues.Count == 0
                ? string.Empty
                : $"&{string.Join("&", this.RouteValues.Select(r => $"{r.Key}={r.Value}"))}";

            content.AppendLine($"<li class=\"{this.PageItemClass}{activePage}\">");
            content.AppendLine($"<a class=\"{this.PageLinkClass}\" href=\"{this.GetLinkAddress()}?page={page}{routeValues}\">{page}{currentPage}</a>");
            content.AppendLine("</li>");
        }

        private void SetPaginatorClasses(TagHelperOutput output)
        {
            var classAttr = output.Attributes
                .FirstOrDefault(a => a.Name == "class");

            if (classAttr is null)
            {
                return;
            }

            output.Attributes.SetAttribute("class", classAttr.Value);
        }
    }
}
