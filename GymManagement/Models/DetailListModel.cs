using System.Collections.Generic;

namespace GymManagement.PL.Models
{
    public class DetailListModel
    {
        public List<DetailItem> Items { get; set; } = new();
    }

    public class DetailItem
    {
        public string Key { get; set; } = "";
        public object Value { get; set; } = "";
    }
}