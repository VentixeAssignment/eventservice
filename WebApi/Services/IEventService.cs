using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Services;

public interface IEventService
{
    Task<EventModel> CreateAsync(EventRegForm dto);
    Task<IEnumerable<EventModel>> GetAllAsync();
    Task<EventModel> GetOneAsync(string id);
    Task<EventModel> UpdateAsync(EventModel model);
    Task<bool> DeleteAsync(string id);
}
