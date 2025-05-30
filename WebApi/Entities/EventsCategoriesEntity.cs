using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Protos;

namespace WebApi.Entities;

public class EventsCategoriesEntity
{
    public string EventId { get; set; } = null!;
    public string CategoryId { get; set; } = null!;



    [ForeignKey(nameof(EventId))]
    public EventEntity Event { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public CategoryEntity Category { get; set; } = null!;
}
