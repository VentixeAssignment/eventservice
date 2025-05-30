using WebApi.Data;
using WebApi.Dto;
using WebApi.Entities;
using WebApi.Factories;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public class EventService(EventRepository repository, ILogger<EventModel> logger, DataContext context) : IEventService
{
    private readonly EventRepository _repository = repository;
    private readonly ILogger<EventModel> _logger = logger;

    private readonly DataContext _context = context;


    public async Task<Result<EventModel>> CreateAsync(EventRegForm dto)
    {
        if (dto == null)
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            var newEntityResult = await _repository.CreateAsync(dto);

            if (!newEntityResult.Success || newEntityResult.Data == null)
                return new Result<EventModel> { Success = false, StatusCode = newEntityResult.StatusCode, ErrorMessage = newEntityResult.ErrorMessage };
            

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var newModel = EventFactory.ModelFromEntity(newEntityResult.Data);

            return newModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 201, Data = newModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create event." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Something went wrong creating event. ##### {ex}");
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating event. ##### {ex}" };
        }
    }

    public async Task<Result<EventModel>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();

        if (!result.Success || result.DataList == null)
            return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        
        var categories = result.DataList.Select(x => EventFactory.ModelFromEntity(x)).ToList();

        return categories.Any()
            ? new Result<EventModel> { Success = true, DataList = categories }
            : new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = "No events was found." };
    }

    public async Task<Result<EventModel>> GetOneAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Id can not be null or empty." };

        var result = await _repository.GetOneAsync(x => x.Id == id);

        if (!result.Success || result.Data == null)
            return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        

        var category = EventFactory.ModelFromEntity(result.Data);

        return category != null
            ? new Result<EventModel> { Success = true, StatusCode = 200, Data = category }
            : new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = $"No Event with id {id} was found." };
    }

    public async Task<Result<EventModel>> UpdateAsync(EventModel model)
    {
        if (model == null)
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            var entityToUpdate = EventFactory.EntityFromModel(model);
            var result = _repository.Update(entityToUpdate);

            if (!result.Success)
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var updatedModel = EventFactory.ModelFromEntity(entityToUpdate);

            return updatedModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Failed to update event. ##### {ex}");
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating event. ##### {ex}" };
        }
    }

    public async Task<Result<EventModel>> UpdateSeatsLeftAsync(string eventId, int seats)
    {
        if (seats == 0 || string.IsNullOrEmpty(eventId))
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid data. Either eventId or seats are missing." };
        

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var entity = await _repository.GetOneAsync(x => x.Id == eventId);

            if (entity == null || entity.Data == null)
                return new Result<EventModel> { Success = false, StatusCode = 404, ErrorMessage = $"No entity found with id {eventId}" };
       
            var result = _repository.UpdateSeatsLeft(entity.Data, seats);

            if (!result.Success)
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var updatedModel = EventFactory.ModelFromEntity(entity.Data);
            return updatedModel != null
                ? new Result<EventModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Failed to update seats left for event. ##### {ex}");
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating column for event. ##### {ex}" };
        }
    }

    public async Task<Result<EventModel>> DeleteAsync(string id)
    {
        if (id == null)
            return new Result<EventModel> { Success = false, StatusCode = 400, ErrorMessage = "Id can not be null or empty." };
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var entityResult = await _repository.GetOneAsync(x => x.Id == id);

            if (!entityResult.Success || entityResult.Data == null)
                return new Result<EventModel> { Success = false, StatusCode = entityResult.StatusCode, ErrorMessage = entityResult.ErrorMessage };

            var result = _repository.DeleteAsync(entityResult.Data);

            if (!result.Success || result.Data == null)
                return new Result<EventModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new Result<EventModel> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Failed to delete entity. ##### {ex}");
            return new Result<EventModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong deleting event. ##### {ex}" };
        }
    }
}