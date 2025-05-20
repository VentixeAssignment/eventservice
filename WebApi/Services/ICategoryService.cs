using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Services
{
    public interface ICategoryService
    {
        Task<Result<CategoryModel>> CreateAsync(CategoryRegForm dto);
        Task<Result<CategoryModel>> GetAllAsync();
        Task<Result<CategoryModel>> GetOneAsync(string id);
        Task<Result<CategoryModel>> UpdateAsync(CategoryModel model);
        Task<Result<CategoryModel>> DeleteAsync(string id);
    }
}
