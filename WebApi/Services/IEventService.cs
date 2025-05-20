using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Services;

public interface IEventService
{
    Task<Result<EventModel>> CreateAsync(EventRegForm dto);
    Task<Result<EventModel>> GetAllAsync();
    Task<Result<EventModel>> GetOneAsync(string id);
    Task<Result<EventModel>> UpdateAsync(EventModel model);
    Task<Result<EventModel>> DeleteAsync(string id);
    Task<Result<EventModel>> UpdateSeatsLeftAsync(string eventId, int seats);
}
