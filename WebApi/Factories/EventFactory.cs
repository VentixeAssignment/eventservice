using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;
using static Grpc.Core.Metadata;

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
            PricePerSeat = dto.Price,
            Currency = dto.Currency,
            TotalSeats = dto.TotalSeats,
            SeatsLeft = dto.TotalSeats
        };
    }

    public static EventModel ModelFromEntity(EventEntity entity)
    {
        return new EventModel
        {
            Id = entity.Id,
            EventImageUrl = entity.EventImageUrl,
            EventName = entity.EventName,
            Description = entity.Description,
            Venue = entity.Venue,
            StreetAddress = entity.StreetAddress,
            PostalCode = entity.PostalCode,
            City = entity.City,
            Country = entity.Country,
            Start = entity.Start,
            End = entity.End,
            Price = entity.PricePerSeat,
            Currency = entity.Currency,
            TotalSeats = entity.TotalSeats,
            SeatsLeft = entity.SeatsLeft ?? 0,
            Categories = entity.EventsCategories.Select(x => new CategoryModel
            {
                Id = x.CategoryId,
                CategoryName = x.Category?.CategoryName ?? ""
            }).ToList()
        };
    }

    public static EventEntity EntityFromModel(EventModel model)
    {
        return new EventEntity
        {
            Id = model.Id,
            EventImageUrl = model.EventImageUrl,
            EventName = model.EventName,
            Description = model.Description,
            Venue = model.Venue,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City,
            Country = model.Country,
            Start = model.Start,
            End = model.End,
            PricePerSeat = model.Price,
            Currency = model.Currency,
            TotalSeats = model.TotalSeats,
            SeatsLeft = model.SeatsLeft,
            EventsCategories = model.Categories.Select(x => new EventsCategoriesEntity
            {
                EventId = model.Id,
                CategoryId = x.Id
            }).ToList()
        };
    }
}
