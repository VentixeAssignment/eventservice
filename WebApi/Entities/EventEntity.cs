﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class EventEntity 
{
    private string _profileImageUrl = "/Images/standard-user-avatar.jpg";


    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string EventImageUrl
    {
        get => _profileImageUrl;
        set => _profileImageUrl = string.IsNullOrWhiteSpace(value)
            ? "/standard-event-image.jpg"
            : value;
    }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string EventName { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(30)")]
    public string Venue { get; set; } = null!;


    [Column(TypeName = "nvarchar(50)")]
    public string? StreetAddress { get; set; } 


    [Column(TypeName = "nvarchar(10)")]
    public string? PostalCode { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string City { get; set; } = null!;


    [Column(TypeName = "nvarchar(20)")]
    public string? Country { get; set; } 

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Start { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime End { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PricePerSeat { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "varchar(3)")]
    public string Currency {  get; set; } = null!;

    [Required]
    public int TotalSeats { get; set; }

    public int? SeatsLeft { get; set; }


    public ICollection<EventsCategoriesEntity> EventsCategories { get; set; } = [];
    
}
