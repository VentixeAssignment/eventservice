using WebApi.Data;
using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Repositories;

public class EventRepository(DataContext context, ILogger<BaseRepository<EventEntity, EventRegForm>> logger) : BaseRepository<EventEntity, EventRegForm>(context, logger)
{
}
