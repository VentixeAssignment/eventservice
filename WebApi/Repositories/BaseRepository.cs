using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repositories;

public class BaseRepository<TEntity, TModel> 
    where TEntity : class
    where TModel : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _table;
    protected readonly ILogger<BaseRepository<TEntity, TModel>> _logger;

    public BaseRepository(DataContext context, ILogger<BaseRepository<TEntity, TModel>> logger)
    {
        _context = context;
        _table = _context.Set<TEntity>();
        _logger = logger;
    }


    #region CRUD

    public virtual async Task<Result<TEntity>> CreateAsync(TEntity entity)
    {
        if (entity == null)
            return new Result<TEntity> { Success = false, ErrorMessage = "Invalid data." };

        try
        {
            var result = await _context.AddAsync(entity);

            return result != null
                ? new Result<TEntity> { Success = true, StatusCode = 201, Data = result.Entity }
                : new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create event." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to create entity of type {EntityType}", typeof(TEntity).Name);
            return new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating entity. ##### {ex}" };
        }
    }

    public async Task<Result<TEntity>> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (expression == null)
            return new Result<TEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid expression input." };

        var exists = await _table.FirstOrDefaultAsync(expression);

        return exists != null
            ? new Result<TEntity> { Success = true, StatusCode = 200 }
            : new Result<TEntity> { Success = false, StatusCode = 404, ErrorMessage = $"No entity found matching {expression}." };
    }

    public async Task<Result<TEntity>> GetAllAsync()
    {
        var entities = await _table.ToListAsync();

        return entities.Count > 0
            ? new Result<TEntity> { Success = true, StatusCode = 200, DataList = entities }
            : new Result<TEntity> { Success = false, StatusCode = 404, ErrorMessage = "No entities were found." };
    }

    public async Task<Result<TEntity>> GetOneAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (expression == null)
            return new Result<TEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid expression." };

        var result = await _table.FirstOrDefaultAsync(expression);

        return result != null
            ? new Result<TEntity> { Success = true, StatusCode = 200, Data = result }
            : new Result<TEntity> { Success = false, StatusCode = 404, ErrorMessage = "No entity found matching input expression." };
    }

    public Result<TEntity> Update(TEntity entity)
    {
        if (entity == null)
            return new Result<TEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid input data." };

        try
        {
            var result = _table.Update(entity);

            return result == null
                ? new Result<TEntity> { Success = true, StatusCode = 200, Data = result?.Entity }
                : new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to update entity." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to update entity of type {EntityType}", typeof(TEntity).Name);
            return new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = "Something went wrong updating entity." };
        }
    }

    public Result<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity == null)
            return new Result<TEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid input." };

        try
        {
            var result = _table.Remove(entity);

            return result != null
                ? new Result<TEntity> { Success = true, StatusCode = 200 }
                : new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to delete entity." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to update entity of type {EntityType}", typeof(TEntity).Name);
            return new Result<TEntity> { Success = false, StatusCode = 500, ErrorMessage = "Something went wrong deleting entity." };
        }
    }

    #endregion
    
}
