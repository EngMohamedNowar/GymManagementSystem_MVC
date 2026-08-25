using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class FormActionsModel
    {
        public FormAction BackAction { get; set; } = new()
        {
            Text = "Back to List",
            Icon = "bi bi-arrow-left",
            CssClass = "btn-outline-secondary rounded-pill px-4"
        };
        public FormAction SubmitAction { get; set; } = new()
        {
            Text = "Save",
            Icon = "bi bi-check-circle",
            CssClass = "btn-fill rounded-pill px-4"
        };
        public List<FormAction> CustomActions { get; set; } = new();
    }

    public class FormAction
    {
        public string Text { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public object RouteValues { get; set; } = null!;
        public string CssClass { get; set; } = "";
    }
}