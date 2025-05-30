using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class EventModel
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string EventImageUrl { get; set; } = null!;

    [Required]
    public string EventName { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Venue { get; set; } = null!;

    public string? StreetAddress { get; set; } 

    public string? PostalCode { get; set; } 

    [Required]
    public string City { get; set; } = null!;

    public string? Country { get; set; } 

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Currency { get; set; } = null!;

    [Required]
    public int TotalSeats { get; set; }

    [Required]
    public int SeatsLeft { get; set; }


    public List<CategoryModel> Categories { get; set; } = [];
}
