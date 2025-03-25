using ChatWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersChatsController : ControllerBase
    {
        private readonly IAllChatsService _chatSearchService;

        public UsersChatsController(IAllChatsService chatSearchService)
        {
            _chatSearchService = chatSearchService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchChats(int id)
        {
            var result = await _chatSearchService.GetChatsListAsync(id);
            
            return Ok(result);
        }
    }
}
