using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace GymManagement.PL.Models
{
    public class ActionButtonsModel
    {
        public object Item { get; set; } = null!;
        public List<ActionButton> Buttons { get; set; } = new();
    }

    public abstract class ActionButton
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string CssClass { get; set; } = "btn-icon";
        public abstract IHtmlContent Render(object item);
    }

    public class LinkActionButton : ActionButton
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string IdProperty { get; set; } = "Id";

        public override IHtmlContent Render(object item)
        {
            var id = item.GetType().GetProperty(IdProperty)?.GetValue(item);
            var url = $"/{Controller}/{Action}/{id}";
            var tag = new TagBuilder("a");
            tag.Attributes["href"] = url;
            tag.Attributes["class"] = CssClass;
            tag.Attributes["title"] = Title;
            tag.InnerHtml.AppendHtml($"<i class=\"{Icon}\"></i>");
            return tag;
        }
    }

    public class FormActionButton : ActionButton
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string IdProperty { get; set; } = "Id";
        public string Method { get; set; } = "post";

        public override IHtmlContent Render(object item)
        {
            var id = item.GetType().GetProperty(IdProperty)?.GetValue(item);
            var form = new TagBuilder("form");
            form.Attributes["asp-controller"] = Controller;
            form.Attributes["asp-action"] = Action;
            form.Attributes["asp-route-id"] = id?.ToString() ?? "";
            form.Attributes["method"] = Method;
            form.Attributes["class"] = "d-inline";

            var btn = new TagBuilder("button");
            btn.Attributes["type"] = "submit";
            btn.Attributes["class"] = CssClass;
            btn.Attributes["title"] = Title;
            btn.InnerHtml.AppendHtml($"<i class=\"{Icon}\"></i>");

            form.InnerHtml.AppendHtml(btn);
            return form;
        }
    }
}