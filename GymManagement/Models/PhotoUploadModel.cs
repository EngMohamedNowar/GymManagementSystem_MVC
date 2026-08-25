using System.Linq.Expressions;

namespace GymManagement.PL.Models
{
    public class PhotoUploadModel
    {
        public LambdaExpression Expression { get; set; } = null!;
        public string ColumnClass { get; set; } = "col-12";
        public string InputId { get; set; } = "photoInput";
        public string PreviewId { get; set; } = "photoPreview";
    }
}