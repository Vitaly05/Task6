using Task6.Models;

namespace Task6.Repositories
{
    public interface IChatRepository
    {
        Task AddNewTagsAsync(IEnumerable<Tag> tags);

        Task AddMessageAsync(Message message);

        Task<IEnumerable<Message>> GetMessagesAsync(IEnumerable<Tag> tags);
    }
}
