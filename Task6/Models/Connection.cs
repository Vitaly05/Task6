namespace Task6.Models
{
    public class Connection
    {
        public string ConnectionId { get; init; }

        public List<string> Tags { get; set; } = new List<string>();

        public Connection(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public bool HasTag(IEnumerable<string> tags)
        {
            if (tags.Count() == 0) return true;
            return Tags.Intersect(tags).Any();
        }
    }
}
