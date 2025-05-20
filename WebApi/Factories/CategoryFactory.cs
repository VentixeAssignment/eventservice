using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Factories;

public static class CategoryFactory
{
    public static CategoryEntity EntityFromDto(CategoryRegForm dto)
    {
        return new CategoryEntity
        {
            CategoryName = dto.CategoryName
        };
    }

    public static CategoryModel ModelFromEntity(CategoryEntity entity)
    {
        return new CategoryModel
        {
            Id = entity.Id,
            CategoryName = entity.CategoryName,
            Events = entity.EventsCategories.Select(x => new EventModel
            {
                Id = x.EventId,
                EventName = x.Event.EventName,
                Description = x.Event.Description,
                Venue = x.Event.Venue,
                StreetAddress = x.Event.StreetAddress,
                PostalCode = x.Event.PostalCode,
                City = x.Event.City,
                Country = x.Event.Country,
                Start = x.Event.Start,
                Price = x.Event.Price,
                Currency = x.Event.Currency,
                SeatsLeft = x.Event.SeatsLeft ?? 0
            }).ToList()
        };
    }

    public static CategoryEntity EntityFromModel(CategoryModel model)
    {
        return new CategoryEntity
        {
            Id = model.Id,
            CategoryName = model.CategoryName,
            EventsCategories = model.Events.Select(x => new EventsCategoriesEntity
            {
                EventId = x.Id,
                CategoryId = model.Id
            }).ToList()
        };
    }

}
