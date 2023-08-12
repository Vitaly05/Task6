using Microsoft.AspNetCore.SignalR;
using Task6.Models;
using Task6.Repositories;

namespace Task6.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRepository chatRepository;

        public ChatHub(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public async Task Send(string message, IEnumerable<string> tags)
        {
            await chatRepository.AddNewTagsAsync(tags);
            await chatRepository.AddMessageAsync(message, tags);
            await Clients.Others.SendAsync("NewMessage");
        }

        public async Task GetMessages(IEnumerable<string> tags)
        {
            await Clients.Caller.SendAsync("GetMessages", await chatRepository.GetMessagesAsync(tags));
        }
    }
}
