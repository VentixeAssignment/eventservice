using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;

        [HttpPost]
        [Route("/create")]
        public async Task<IActionResult> CreateEventAsync(EventRegForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.CreateAsync(form);

            if (result.StatusCode == 409)
                return Conflict(new { result.ErrorMessage });

            if (result.StatusCode == 400)
                return BadRequest(new { result.ErrorMessage });

            return result.Success
                ? Created(string.Empty, new { result.ErrorMessage })
                : StatusCode(result.StatusCode, new { result.ErrorMessage });
        }
    }
}
