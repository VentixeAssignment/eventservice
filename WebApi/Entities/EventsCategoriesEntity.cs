using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class EventsCategoriesEntity 
{
    [Required]
    public string CategoryId { get; set; } = null!;

    [Required]
    public string EventId { get; set; } = null!;


    [ForeignKey(nameof(CategoryId))]
    public CategoryEntity Category { get; set; } = new();

    [ForeignKey(nameof(EventId))]
    public EventEntity Event { get; set; } = new();
}
