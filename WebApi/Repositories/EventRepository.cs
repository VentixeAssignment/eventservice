using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Repositories;

public class EventRepository(DataContext context, ILogger<BaseRepository<EventEntity, EventRegForm>> logger) : BaseRepository<EventEntity, EventRegForm>(context, logger)
{
    public Result<EventEntity> UpdateSeatsLeft(EventEntity entity, int seats)
    {
        if (seats == 0 || entity == null)
        {
            _logger.LogWarning("UpdateSeatsLeft was called without eventId or seats.");
            return new Result<EventEntity> { Success = false, StatusCode = 400, ErrorMessage = "UpdateSeatsLeft was called with invalid id or seats." };
        }

        try
        {
            entity.SeatsLeft -= seats;

            return new Result<EventEntity> { Success = true, StatusCode = 200, Data = entity };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to update column in entity of type {EntityType}", typeof(EventEntity).Name);
            return new Result<EventEntity> { Success = false, StatusCode = 404, 
                ErrorMessage = $"Something went wrong updating column SeatsLeft for event with id: {entity.Id}" };
        }
    }
}
