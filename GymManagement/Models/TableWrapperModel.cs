using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class TableWrapperModel<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public List<TableColumn<T>> Columns { get; set; } = new();
        public bool ShowPagination { get; set; } = true;
        public GymManagementSystem.BLL.ViewModels.Common.PaginationViewModel<T> Pagination { get; set; }
        public EmptyStateModel EmptyState { get; set; } = new();
    }

    public class TableColumn<T>
    {
        public string Title { get; set; } = "";
        public string HeaderCssClass { get; set; } = "";
        public string CellCssClass { get; set; } = "";
        public System.Func<T, object> GetValue { get; set; } = _ => "";
        public System.Func<T, Microsoft.AspNetCore.Html.IHtmlContent> Template { get; set; } = null!;
    }
}