using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Dto;

public class EventRegForm
{

    [Required(ErrorMessage = "Field is required.")]
    public string EventName { get; set; } = null!;

    [Required(ErrorMessage = "Field is required.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Field is required.")]
    public string Venue { get; set; } = null!;

    
    public string? StreetAddress { get; set; }

    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public string City { get; set; } = null!;


    public string? Country { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public DateTime Start { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public DateTime End { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public string Currency { get; set; } = null!;

    [Required(ErrorMessage = "Field is required.")]
    public int TotalSeats { get; set; }

    [Required(ErrorMessage = "Field is required.")]
    public IEnumerable<CategoryModel> Categories { get; set; } = [];
}


