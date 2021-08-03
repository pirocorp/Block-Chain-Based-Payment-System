namespace PaymentSystem.WalletApp.Web.Infrastructure.TagHelpers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement(Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : TagHelper
    {
        public ActiveRouteTagHelper()
        {
            this.Area = string.Empty;
            this.Controller = string.Empty;
            this.Action = string.Empty;
            this.Page = string.Empty;
        }

        [HtmlAttributeName("is-active-route")]
        public string Class { get; set; }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-page")]
        public string Page { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (this.ShouldBeActive())
            {
                this.MakeActive(output);
            }

            output.Attributes.RemoveAll("is-active-route");
        }

        private bool ShouldBeActive()
        {
            var currentArea = this.GetRouteValue("Area");
            var currentController = this.GetRouteValue("Controller");
            var currentAction = this.GetRouteValue("Action");
            var currentPage = this.GetRouteValue("Page");

            var areaIsCorrect = string.Equals(this.Area, currentArea, StringComparison.InvariantCultureIgnoreCase);
            var controllerIsCorrect = string.Equals(this.Controller, currentController, StringComparison.InvariantCultureIgnoreCase);
            var actionIsCorrect = string.Equals(this.Action, currentAction, StringComparison.InvariantCultureIgnoreCase);
            var pageIsCorrect = string.Equals(this.Page, currentPage, StringComparison.InvariantCultureIgnoreCase);

            return areaIsCorrect && controllerIsCorrect && actionIsCorrect && pageIsCorrect;
        }

        private string GetRouteValue(string parameter)
        {
            return this.ViewContext.RouteData.Values[parameter] == null 
                ? string.Empty
                : this.ViewContext.RouteData.Values[parameter].ToString();
        }

        private void MakeActive(TagHelperOutput output)
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
