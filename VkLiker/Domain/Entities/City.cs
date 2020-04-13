using Database;

namespace Domain.Entities
{
    public class City : BaseEntity
    {
        public long SourceId { get; set; }
        public string Title { get; set; }
    }
}