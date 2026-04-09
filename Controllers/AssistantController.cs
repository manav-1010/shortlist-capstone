using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using Shortlist.Web.Services;

namespace Shortlist.Web.Controllers
{
    [ApiController]
    [Route("api/assistant")]
    public class AssistantController : ControllerBase
    {
        private readonly IShortlistAssistantService _assistantService;

        public AssistantController(IShortlistAssistantService assistantService)
        {
            _assistantService = assistantService;
        }

        [HttpPost("ask")]
        public IActionResult Ask([FromBody] ChatMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message is required." });
            }

            var reply = _assistantService.GetReply(request.Message, request.CurrentPage);

            return Ok(new ChatMessageResponse
            {
                Reply = reply
            });
        }
    }
}