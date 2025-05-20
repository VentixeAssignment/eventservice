using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public class EventService(EventRepository repository) : IEventService
{
    private readonly EventRepository _repository = repository;


    public Task<EventModel> CreateAsync(EventRegForm dto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<EventModel>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<EventModel> GetOneAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<EventModel> UpdateAsync(EventModel model)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}