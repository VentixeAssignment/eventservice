using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;


        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetEventsAsync()
        {
            var events = await _eventService.GetAllAsync();
            if (events == null || events.DataList == null)
                return BadRequest("No events returned.");

                return events!.DataList!.Any()
                    ? Ok(events.DataList)
                    : NotFound("No events fount.");
        }


        [HttpPost]
        [Route("create")]
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
                ? Created(string.Empty, new { result.Data })
                : StatusCode(result.StatusCode, new { result.ErrorMessage });
        }
    }
}
