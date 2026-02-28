using System.ComponentModel.DataAnnotations;

namespace SmartHotelManagement.Model;

public class Category : Entity
{
    public int CategoryId { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public CategoryKind Kind { get; set; } = CategoryKind.Expense;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}
