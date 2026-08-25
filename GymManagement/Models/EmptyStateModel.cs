using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class EmptyStateModel
    {
        public string Icon { get; set; } = "bi bi-inbox";
        public string Title { get; set; } = "No Data";
        public string Message { get; set; } = "";
        public EmptyStateAction Action { get; set; } = null!;
    }

    public class EmptyStateAction
    {
        public string Text { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public Dictionary<string, string> RouteValues { get; set; } = new();
        public string CssClass { get; set; } = "btn-fill rounded-pill px-3";
    }
}