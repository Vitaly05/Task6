using Microsoft.AspNetCore.SignalR;
using Task6.Models;
using Task6.Repositories;

namespace Task6.Hubs
{
    public class ChatHub : Hub
    {
        private static List<Connection> connections = new();

        private readonly IChatRepository chatRepository;

        public ChatHub(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public override Task OnConnectedAsync()
        {
            connections.Add(new Connection(Context.ConnectionId));
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            connections.Remove(connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Send(string message, IEnumerable<string> tags)
        {
            await chatRepository.AddNewTagsAsync(tags);
            await chatRepository.AddMessageAsync(message, tags);
            var ids = connections.Where(c => c.ConnectionId != Context.ConnectionId && c.HasTag(tags))
                .Select(c => c.ConnectionId);
            await Clients.Clients(ids).SendAsync("NewMessage", message);
        }

        public async Task GetMessages(IEnumerable<string> tags)
        {
            await Clients.Caller.SendAsync("GetMessages", await chatRepository.GetMessagesAsync(tags));
        }

        public void AddTag(string tagName)
        {
            getCurrentConnection().Tags.Add(tagName);
        }

        public void RemoveTag(string tagName)
        {
            getCurrentConnection().Tags.Remove(tagName);
        }

        private Connection getCurrentConnection() => connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
    }
}
