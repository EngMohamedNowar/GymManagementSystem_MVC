using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class PageHeaderModel
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public List<PageHeaderAction> Actions { get; set; } = new();
    }

    public class PageHeaderAction
    {
        public string Text { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public object RouteValues { get; set; } = null!;
        public string CssClass { get; set; } = "btn-fill rounded-pill px-3 py-2";
        public string Title { get; set; } = "";
    }
}