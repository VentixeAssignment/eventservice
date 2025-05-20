
using WebApi.Dto;
using WebApi.Factories;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public class CategoryService(CategoryRepository repository, ILogger<CategoryModel> logger) : ICategoryService
{
    private readonly CategoryRepository _repository = repository;
    private readonly ILogger<CategoryModel> _logger = logger;


    public async Task<Result<CategoryModel>> CreateAsync(CategoryRegForm dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("CreateAsync method was called with null DTO.");
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            var begin = await _repository.BeginTransactionAsync();
            if (!begin.Success)
            {
                _logger.LogWarning("Failed to begin transaction for creating category: {Error}", begin.ErrorMessage);
                return new Result<CategoryModel> { Success = false, StatusCode = begin.StatusCode, ErrorMessage = begin.ErrorMessage };
            }

            var newEntityResult = await _repository.CreateAsync(dto);

            if (!newEntityResult.Success || newEntityResult.Data == null)
            {
                _logger.LogWarning("Failed to create category in repository: {Error}", newEntityResult.ErrorMessage);
                return new Result<CategoryModel> { Success = false, StatusCode = begin.StatusCode, ErrorMessage = begin.ErrorMessage };
            }

            await _repository.SaveChangesAsync();
            await _repository.CommitTransactionAsync();

            var newModel = CategoryFactory.ModelFromEntity(newEntityResult.Data);
            return newModel != null
                ? new Result<CategoryModel> { Success = true, StatusCode = 201, Data = newModel }
                : new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create category."};
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "Something went wrong creating model of type {ModelType}", typeof(CategoryModel).Name);
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating category.\n{ex.Message}" };
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
        {
            _logger.LogWarning("Update async method was called with invalid data.");
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entityToUpdate = CategoryFactory.EntityFromModel(model);
            var result = _repository.Update(entityToUpdate);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update category in repository {Error}", result.ErrorMessage);
                return new Result<CategoryModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            var updatedModel = CategoryFactory.ModelFromEntity(entityToUpdate);
            return updatedModel != null
                ? new Result<CategoryModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "\nFailed to update for model of type {ModelType}", typeof(CategoryModel).Name);
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating category.\n{ex.Message}" };
        }
    }

    public async Task<Result<CategoryModel>> DeleteAsync(string id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync method was called with null ID.");
            return new Result<CategoryModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid id." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

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

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            return new Result<CategoryModel> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError(ex.Message, "\nFailed to delete model of type {ModelType}", typeof(CategoryModel).Name);
            return new Result<CategoryModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong deleting category.\n{ex.Message}" };
        }
    }
}
