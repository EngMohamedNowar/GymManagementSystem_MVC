using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class SearchFormModel
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public string SearchLabel { get; set; } = "Search";
        public string SearchPlaceholder { get; set; } = "";
        public string SearchValue { get; set; } = "";
        public string SubmitText { get; set; } = "Search";
        public string ClearText { get; set; } = "Clear";
        public List<SearchFilter> Filters { get; set; } = new();
    }

    public class SearchFilter
    {
        public string Name { get; set; } = "";
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public List<SelectOption> Options { get; set; } = new();
    }

    public class SelectOption
    {
        public string Value { get; set; } = "";
        public string Text { get; set; } = "";
    }
}