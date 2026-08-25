using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Models
{
    public class FormSectionModel
    {
        public string Title { get; set; } = "";
        public string TitleCssClass { get; set; } = "";
        public List<FormField> Fields { get; set; } = new();
    }

    public class FormField
    {
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public string ColumnClass { get; set; } = "col-12 col-md-6";
        public string Placeholder { get; set; } = "";
        public LambdaExpression Expression { get; set; } = null!;
        public bool IsSelect { get; set; } = false;
        public bool IsTextArea { get; set; } = false;
        public int Rows { get; set; } = 3;
        public string EmptyOptionText { get; set; } = "";
        public IEnumerable<SelectListItem> SelectItems { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}