using Task6.Models;

namespace Task6.Repositories
{
    public interface IChatRepository
    {
        Task AddNewTagsAsync(IEnumerable<string> tagsNames);

        Task AddMessageAsync(string message, IEnumerable<string> tags);

        Task<IEnumerable<Message>> GetMessagesAsync(IEnumerable<string> tagsNames);
    }
}
