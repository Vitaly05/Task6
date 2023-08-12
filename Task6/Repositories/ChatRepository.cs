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

        public async Task AddMessageAsync(Message message)
        {
            //var messageTagsNames = new List<string>(message.Tags.Select(t => t.Name));
            //message.Tags.Clear();
            //await addTagToMessage(message, messageTagsNames);
            message.Tags = await getTagsAsync(message.Tags.Select(t => t.Name));
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }

        async public Task AddNewTagsAsync(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
                if (!context.Tags.Any(t => t.Name == tag.Name))
                    await context.AddAsync(tag);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(IEnumerable<Tag> tags)
        {
            List<Message> messages = new List<Message>();
            List<Tag> usedTags = await getTagsAsync(tags.Select(t => t.Name));
            foreach (var message in context.Messages.Include(m => m.Tags))
            {
                if (message.Tags.Count() == 0 || message.Tags.Intersect(usedTags).Any())
                    messages.Add(message);
            }
            messages.ForEach(m => (m.Tags as List<Tag>).ForEach(t => t.Messages.Clear()));
            return messages;
        }

        private async Task addTagToMessage(Message message, IEnumerable<string> tagsNames)
        {
            foreach (var tagName in tagsNames)
                message.Tags.Add(await getTagAsync(tagName));
        }

        private async Task<List<Tag>> getTagsAsync(IEnumerable<string> tagsNames)
        {
            List<Tag> tags = new List<Tag>();
            foreach (var tagName in tagsNames)
                tags.Add(await getTagAsync(tagName));
            return tags;
        }

        private async Task<Tag> getTagAsync(string tagName) => await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
    }
}
