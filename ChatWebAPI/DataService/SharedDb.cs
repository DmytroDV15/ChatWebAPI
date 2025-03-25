using ChatWebAPI.Models;
using System.Collections.Concurrent;

namespace ChatWebAPI.DataService
{
    public class SharedDb
    {
        public ConcurrentDictionary<string, ChatMessageModel> Connections { get; set; } = new ConcurrentDictionary<string, ChatMessageModel>();
    }
}
