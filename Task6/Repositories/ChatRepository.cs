using Microsoft.EntityFrameworkCore;
using Task6.Models;

namespace Task6.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationContext context;

        public ChatRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task AddMessageAsync(string message, IEnumerable<string> tags)
        {
            var newMessage = new Message()
            {
                Data = message,
                Tags = await getTagsAsync(tags)
            };
            await context.Messages.AddAsync(newMessage);
            await context.SaveChangesAsync();
        }

        async public Task AddNewTagsAsync(IEnumerable<string> tagsNames)
        {
            foreach (var tagName in tagsNames)
                if (!context.Tags.Any(t => t.Name == tagName))
                    await context.AddAsync(new Tag() { Name = tagName });
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(IEnumerable<string> tagsNames)
        {
            List<Tag> usedTags = await getTagsAsync(tagsNames);
            List<Message> messages = getMessages(usedTags);
            messages.ForEach(m => (m.Tags as List<Tag>).ForEach(t => t.Messages.Clear()));
            return messages;
        }

        private List<Message> getMessages(IEnumerable<Tag> tags)
        {
            List<Message> messages = new List<Message>();
            foreach (var message in context.Messages.Include(m => m.Tags))
                if (message.Tags.Count() == 0 || message.Tags.Intersect(tags).Any())
                    messages.Add(message);
            return messages;
        }

        private async Task<List<Tag>> getTagsAsync(IEnumerable<string> tagsNames)
        {
            List<Tag> tags = new List<Tag>();
            foreach (var tagName in tagsNames)
                tags.Add(await getTagAsync(tagName));
            return tags;
        }

        private async Task<Tag> getTagAsync(string tagName) => 
            await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
    }
}
