using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Factories;

public static class EventFactory
{
    public static EventEntity EntityFromDto(EventRegForm dto)
    {
        return new EventEntity
        {
            EventName = dto.EventName,
            Description = dto.Description,
            Venue = dto.Venue,
            StreetAddress = dto.StreetAddress,
            PostalCode = dto.PostalCode,
            City = dto.City,
            Country = dto.Country,
            Start = dto.Start,
            End = dto.End,
            Price = dto.Price,
            Currency = dto.Currency,
            TotalSeats = dto.TotalSeats
        };
    }

    public static EventModel ModelFromEntity(EventEntity entity)
    {
        return new EventModel
        {
            Id = entity.Id,
            EventName = entity.EventName,
            Description = entity.Description,
            Venue = entity.Venue,
            StreetAddress = entity.StreetAddress,
            PostalCode = entity.PostalCode,
            City = entity.City,
            Country = entity.Country,
            Start = entity.Start,
            End = entity.End,
            Price = entity.Price,
            Currency = entity.Currency,
            TotalSeats = entity.TotalSeats,
            SeatsLeft = entity.SeatsLeft ?? 0,
            Categories = entity.EventsCategories.Select(x => new CategoryModel
            {
                Id = x.CategoryId,
                CategoryName = x.Category.CategoryName
            }).ToList()
        };
    }
}
