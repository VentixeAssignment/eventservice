using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class CategoryRegForm
{
    [Required(ErrorMessage = "Field is required.")]
    [RegularExpression(@"^.{2,20}$", ErrorMessage = "Category name must be between 2 and 20 characters.")]
    public string CategoryName { get; set; } = null!;
}
