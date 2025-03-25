using ChatWebAPI.Models;
using ChatWebAPI.Repository;
using ChatWebAPI.Services;
using ChatWebAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace ChatWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IRepository<ChatMessageModel> _messageRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<RegistrationModel> _userRepository;
        private readonly TextAnalyticsService _textAnalyticsService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(
            IRepository<ChatMessageModel> messageRepository,
            IRepository<Chat> chatRepository,
            IRepository<RegistrationModel> userRepository,
            TextAnalyticsService textAnalyticsService,
            IHubContext<ChatHub> hubContext)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _textAnalyticsService = textAnalyticsService;
            _hubContext = hubContext;
        }

        
        [HttpPost("create")]
        public async Task<IActionResult> CreateChat(JoinChatDto conn)
        {
            var user = await _userRepository.GetByIdAsync(conn.RegisterModelId);
            if (user == null) return NotFound("User not found.");

            var existingChat = await _chatRepository.FindAsync(c => c.ChatName == conn.ChatName);
            if (existingChat != null) return BadRequest("Chat already exists.");

            var chatRoom = new Chat
            {
                ChatName = conn.ChatName,
                CreatorId = conn.RegisterModelId,
                Creator = user,
                Users = new List<RegistrationModel> { user }
            };

            await _chatRepository.AddAsync(chatRoom);
            await _chatRepository.SaveAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "admin",
                $"{user.UserName} created {conn.ChatName}", null, chatRoom.Id);

            return Ok(chatRoom);
        }

        [HttpPost("join")]
        public async Task JoinChat(JoinChatDto conn)
        {
            var user = await _userRepository.GetByIdAsync(conn.RegisterModelId);
            if (user == null) throw new Exception("Chat room not found.");

            var chatRoom = await _chatRepository.GetChatByName(conn.ChatName);
            if (chatRoom == null) throw new Exception("User not found.");

            bool isUserInChat = chatRoom.Users.Any(u => u.Id == conn.RegisterModelId);

            if (!isUserInChat)
            {
                chatRoom.Users.Add(user);
                await _chatRepository.SaveAsync();
            }

            // Повідомлення надсилається в будь-якому випадку
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "admin",
                $"{user.UserName} joined {conn.ChatName}", null, chatRoom.Id);

            
        }



        [HttpPost("send")]
        public async Task SendMessage(int chatId, string msg, string token)
        {
            var chatRoom = await _chatRepository.GetByIdAsync(chatId);
            if (chatRoom == null) throw new Exception("Chat room not found."); ;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception("User not found.");

            var sentiment = _textAnalyticsService.AnalyzeSentiment(msg);
            var chatMessage = new ChatMessageModel
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatRoom.Id,
                MessageText = msg,
                Date = DateTime.UtcNow,
                MessageSentimet = sentiment,
                Chat = chatRoom
            };

            await _messageRepository.AddAsync(chatMessage);
            await _messageRepository.SaveAsync();

            await _hubContext.Clients.Group(chatRoom.ChatName).SendAsync("ReceiveSpecificMessage", user.UserName, msg, sentiment, chatRoom.Id);
            
        }
    }
}
