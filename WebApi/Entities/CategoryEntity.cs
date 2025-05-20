using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class CategoryEntity 
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string CategoryName { get; set; } = null!;


    public ICollection<EventsCategoriesEntity> EventsCategories { get; set; } = [];
}
