using Task6.Models;

namespace Task6.Repositories
{
    public interface IChatRepository
    {
        Task<bool> AddNewTagsAsync(IEnumerable<string> tagsNames);

        Task<IEnumerable<string>> GetAllTagsAsync();

        Task AddMessageAsync(string message, IEnumerable<string> tags);

        Task<IEnumerable<Message>> GetMessagesAsync(IEnumerable<string> tagsNames);
    }
}
