
namespace Domain.Entities
{
    public class City : ExternalEntity
    {
        public string Title { get; set; }
        public long RegionId { get; set; }
        public Region Region { get; set; }
    }
}