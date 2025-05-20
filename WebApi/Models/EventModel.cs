using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Entities;

namespace WebApi.Models;

public class EventModel
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string EventName { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Venue { get; set; } = null!;

    [Required]
    public string StreetAddress { get; set; } = null!;

    [Required]
    public string PostalCode { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;

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


    public IEnumerable<CategoryModel> Categories { get; set; } = [];
}
