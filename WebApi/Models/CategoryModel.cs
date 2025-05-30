namespace WebApi.Models;

public class CategoryModel
{
    public string Id { get; set; } = null!;
    public string? CategoryName { get; set; }

    public IEnumerable<EventModel>? Events { get; set; } = [];
}
