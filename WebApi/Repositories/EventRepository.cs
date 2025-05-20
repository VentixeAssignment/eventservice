using WebApi.Data;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Repositories;

public class EventRepository(DataContext context, ILogger<BaseRepository<EventEntity, EventModel>> logger) : BaseRepository<EventEntity, EventModel>(context, logger)
{
}
