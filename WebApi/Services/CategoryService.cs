
using Grpc.Core;
using System.Transactions;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Factories;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public class CategoryService(CategoryRepository repository, ILogger<CategoryModel> logger, DataContext context) : ICategoryService
{
    private readonly CategoryRepository _repository = repository;
    private readonly ILogger<CategoryModel> _logger = logger;
    private readonly DataContext _context = context;


    public async Task<Result<CategoryModel>> CreateAsync(CategoryRegForm dto)
    {
        if (dto == null)
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };

        var exists = await _repository.ExistsAsync(x => x.CategoryName == dto.CategoryName);

        if (exists.Success)
            return new Result<CategoryModel> { Success = false, StatusCode = exists.StatusCode, ErrorMessage = $"A category with the name {dto.CategoryName} already exists." };

        var entity = CategoryFactory.EntityFromDto(dto);

        using var transaction = await _context.Database.BeginTransactionAsync(); 
        try
        {
            var newEntityResult = await _repository.CreateAsync(entity);

            if (!newEntityResult.Success || newEntityResult.Data == null)
            {
                await transaction.RollbackAsync();
                return new Result<CategoryModel> { Success = false, StatusCode = newEntityResult.StatusCode, ErrorMessage = newEntityResult.ErrorMessage };
            }


            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var newModel = CategoryFactory.ModelFromEntity(newEntityResult.Data);

            return newModel != null
                ? new Result<CategoryModel> { Success = true, StatusCode = 201, Data = newModel }
                : new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create category." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Something went wrong creating category. ##### {ex}");
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating category. ##### {ex}" };
        }
    }

    public async Task<Result<CategoryModel>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        if (!result.Success || result.DataList == null)
        {
            _logger.LogWarning("Failed to fetch categories {Error}", result.ErrorMessage);
            return new Result<CategoryModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var categories = result.DataList.Select(x => CategoryFactory.ModelFromEntity(x)).ToList();

        return categories.Any()
            ? new Result<CategoryModel> { Success = true, DataList = categories }
            : new Result<CategoryModel> { Success = false, StatusCode = 404, ErrorMessage = "No categories was found." };
    }

    public async Task<Result<CategoryModel>> GetOneAsync(string id)
    {
        var result = await _repository.GetOneAsync(x => x.Id == id);
        if (!result.Success || result.Data == null)
        {
            _logger.LogWarning("Failed to fetch category {Error}", result.ErrorMessage);
            return new Result<CategoryModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var category = CategoryFactory.ModelFromEntity(result.Data);

        return category != null
            ? new Result<CategoryModel> { Success = true, StatusCode = 200, Data = category }
            : new Result<CategoryModel> { Success = false, StatusCode = 404, ErrorMessage = $"No category with id {id} was found." };
    }

    public async Task<Result<CategoryModel>> UpdateAsync(CategoryModel model)
    {
        if (model == null)
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };

        var entityToUpdate = CategoryFactory.EntityFromModel(model);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var result = _repository.Update(entityToUpdate);

            if (!result.Success)
                return new Result<CategoryModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var updatedModel = CategoryFactory.ModelFromEntity(entityToUpdate);
            return updatedModel != null
                ? new Result<CategoryModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Something went wrong updating category. ##### {ex}");
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating category. ##### {ex}" };
        }
    }

    public async Task<Result<CategoryModel>> DeleteAsync(string id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync method was called with null ID.");
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid id." };
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            var entityResult = await _repository.GetOneAsync(x => x.Id == id);
            if (!entityResult.Success || entityResult.Data == null)
            {
                _logger.LogWarning("Failed to fetch category to delete from repository {Error}", entityResult.ErrorMessage);
                return new Result<CategoryModel> { Success = false, StatusCode = entityResult.StatusCode, ErrorMessage = entityResult.ErrorMessage };
            }

            var result = _repository.DeleteAsync(entityResult.Data);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Failed to delete category {Error}", result.ErrorMessage);
                return new Result<CategoryModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new Result<CategoryModel> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Something went wrong deleting category. ##### {ex}");
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong deleting category. ##### {ex}" };
        }
    }
}
