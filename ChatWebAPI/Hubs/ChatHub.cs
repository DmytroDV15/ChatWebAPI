using ChatWebAPI.DataService;
using ChatWebAPI.Models;
using ChatWebAPI.Repository;
using ChatWebAPI.Services;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace ChatWebAPI.Hubs
{
    public class ChatHub : Hub
    {

        public async Task JoinChatRoom(JoinChatDto conn)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, conn.ChatName);
            await Clients.Group(conn.ChatName).SendAsync("JoinSpecificChatRoom", conn.RegisterModelId, "joined", conn.ChatName);
        }


        public async Task SendMessageToGroup(string chatName, string userName, string message, string sentiment, int chatId)
        {
            await Clients.Group(chatName).SendAsync("ReceiveSpecificMessage", userName, message, sentiment, chatId);
        }
    }
}