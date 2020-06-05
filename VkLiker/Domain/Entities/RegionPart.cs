
namespace Domain.Entities
{
    public class RegionPart : ExternalEntity
    {
        public string Title { get; set; }
        public long RegionId { get; set; }
        public VkRegion VkRegion { get; set; }
    }
}