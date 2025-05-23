using WebApi.Dto;
using WebApi.Entities;
using WebApi.Factories;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public class EventService(EventRepository repository, ILogger<EventModel> logger) : IEventService
{
    private readonly EventRepository _repository = repository;
    private readonly ILogger<EventModel> _logger = logger;


    public async Task<Result<EventModel>> CreateAsync(EventRegForm dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("CreateAsync method was called with null DTO.");
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var newEntityResult = await _repository.CreateAsync(dto);

            if (!newEntityResult.Success || newEntityResult.Data == null)
            {
                _logger.LogWarning("Failed to create event in repository: {Error}", newEntityResult.ErrorMessage);
                return new Result<EventModel> { Success = false, StatusCode = newEntityResult.StatusCode, ErrorMessage = newEntityResult.ErrorMessage };
            }

            await _repository.SaveChangesAsync();
            await _repository.CommitTransactionAsync();

            var newModel = EventFactory.ModelFromEntity(newEntityResult.Data);
            return newModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 201, Data = newModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create event." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "Something went wrong creating event of type {ModelType}", typeof(EventModel).Name);
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating event.\n{ex.Message}" };
        }
    }

    public async Task<Result<EventModel>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        if (!result.Success || result.DataList == null)
        {
            _logger.LogWarning("Failed to fetch events {Error}", result.ErrorMessage);
            return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var categories = result.DataList.Select(x => EventFactory.ModelFromEntity(x)).ToList();

        return categories.Any()
            ? new Result<EventModel> { Success = true, DataList = categories }
            : new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = "No events was found." };
    }

    public async Task<Result<EventModel>> GetOneAsync(string id)
    {
        var result = await _repository.GetOneAsync(x => x.Id == id);
        if (!result.Success || result.Data == null)
        {
            _logger.LogWarning("Failed to fetch event {Error}", result.ErrorMessage);
            return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var category = EventFactory.ModelFromEntity(result.Data);

        return category != null
            ? new Result<EventModel> { Success = true, StatusCode = 200, Data = category }
            : new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = $"No Event with id {id} was found." };
    }

    public async Task<Result<EventModel>> UpdateAsync(EventModel model)
    {
        if (model == null)
        {
            _logger.LogWarning("Update async method was called with invalid data.");
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entityToUpdate = EventFactory.EntityFromModel(model);
            var result = _repository.Update(entityToUpdate);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update event in repository {Error}", result.ErrorMessage);
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            var updatedModel = EventFactory.ModelFromEntity(entityToUpdate);
            return updatedModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "\nFailed to update for model of type {ModelType}", typeof(EventModel).Name);
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating event.\n{ex.Message}" };
        }
    }

    public async Task<Result<EventModel>> UpdateSeatsLeftAsync(string eventId, int seats)
    {
        if (seats == 0 || string.IsNullOrEmpty(eventId))
        {
            _logger.LogWarning("UpdateSeatsLeftAsync method was called with invalid data.");
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid data. Either eventId or seats are missing." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entity = await _repository.GetOneAsync(x => x.Id == eventId);
            if (entity == null || entity.Data == null)
            {
                _logger.LogWarning($"No entity found matching event with id {eventId}");
                return new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = $"No entity found with id {eventId}" };
            }

            var result = _repository.UpdateSeatsLeft(entity.Data, seats);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update column for event in repository {Error}", result.ErrorMessage);
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            var updatedModel = EventFactory.ModelFromEntity(entity.Data);
            return updatedModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "\nFailed to update column for model of type {ModelType}", typeof(EventModel).Name);
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating column for event.\n{ex.Message}" };
        }
    }

    public async Task<Result<EventModel>> DeleteAsync(string id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync method was called with null ID.");
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid id." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entityResult = await _repository.GetOneAsync(x => x.Id == id);
            if (!entityResult.Success || entityResult.Data == null)
            {
                _logger.LogWarning("Failed to fetch event to delete from repository {Error}", entityResult.ErrorMessage);
                return new Result<EventModel> { Success = false, StatusCode = entityResult.StatusCode, ErrorMessage = entityResult.ErrorMessage };
            }

            var result = _repository.DeleteAsync(entityResult.Data);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Failed to delete event {Error}", result.ErrorMessage);
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            return new Result<EventModel> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "\nFailed to delete model of type {ModelType}", typeof(EventModel).Name);
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong deleting event.\n{ex.Message}" };
        }
    }
}