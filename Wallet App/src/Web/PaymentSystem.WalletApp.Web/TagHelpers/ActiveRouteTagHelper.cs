namespace PaymentSystem.WalletApp.Web.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement(Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IDictionary<string, string> routeValues;

        public ActiveRouteTagHelper(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            
            this.Area = string.Empty;
            this.Controller = string.Empty;
            this.Action = string.Empty;
            this.Page = string.Empty;
        }

        [HtmlAttributeName("is-active-route")]
        public string Class { get; set; }

        /// <summary>
        /// The name of the area.
        /// </summary>
        /// <remarks>Must be <c>null</c> if
        /// <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" />
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName("asp-area")] 
        public string Area { get; set; }

        /// <summary>
        /// The name of the controller.
        /// </summary>
        /// <remarks>Must be <c>null</c> if
        /// <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" />
        /// is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>
        /// The name of the action method.
        /// </summary>
        /// <remarks>Must be <c>null</c> if
        /// <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" />
        /// is non-<c>null</c>.
        /// </remarks>
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
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            
            if (classAttr == null)
            {
                classAttr = new TagHelperAttribute("class", this.Class);
                output.Attributes.Add(classAttr);
            }
            else if (classAttr.Value == null || classAttr.Value.ToString().IndexOf(this.Class) < 0)
            {
                output.Attributes.SetAttribute("class", classAttr.Value == null
                    ? this.Class
                    : classAttr.Value.ToString() + $" {this.Class}");
            }
        }
    }
}
