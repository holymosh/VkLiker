
namespace Domain.Entities
{
    public class City : BaseEntity
    {
        public long SourceId { get; set; }
        public string Title { get; set; }
        public long RegionId { get; set; }
        public Region Region { get; set; }
    }
}