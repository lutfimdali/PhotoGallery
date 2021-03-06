﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PhotoGallery.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "if-authorized")]
    public class AuthorizeTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!ViewContext.HttpContext.User.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
            }
            else if (context.AllAttributes.TryGetAttribute("if-authorized", out var attribute))
            {
                output.Attributes.Remove(attribute);
            }
        }
    }
}
