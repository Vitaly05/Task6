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

        public async Task Send(Message message)
        {
            await chatRepository.AddNewTagsAsync(message.Tags);
            await chatRepository.AddMessageAsync(message);
            await Clients.Others.SendAsync("NewMessage");
            await Clients.All.SendAsync("ff");
            await Clients.All.SendAsync("aa");
        }

        public async Task GetMessages(IEnumerable<Tag> tags)
        {
            await Clients.Caller.SendAsync("GetMessages", await chatRepository.GetMessagesAsync(tags));
        }
    }
}
