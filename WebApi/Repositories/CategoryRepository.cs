using WebApi.Data;
using WebApi.Dto;
using WebApi.Entities;

namespace WebApi.Repositories;

public class CategoryRepository(DataContext context, ILogger<BaseRepository<CategoryEntity, CategoryRegForm>> logger) : BaseRepository<CategoryEntity, CategoryRegForm>(context, logger)
{
}
