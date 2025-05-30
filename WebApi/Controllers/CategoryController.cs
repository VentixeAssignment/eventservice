using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync(CategoryRegForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateAsync(form);

            if (result.StatusCode == 409)
                return Conflict(new { result.ErrorMessage });

            if (result.StatusCode == 400)
                return BadRequest(new { result.ErrorMessage });

            if (result.StatusCode == 500)
                return BadRequest(new { result.ErrorMessage });

            return result.Success
                ? Created(string.Empty, new { result.Data })
                : StatusCode(result.StatusCode, new { result.ErrorMessage });
        }
    }
}
