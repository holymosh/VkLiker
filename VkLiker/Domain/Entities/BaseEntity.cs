using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public abstract class ExternalEntity : BaseEntity
    {
        public long SourceId { get; set; }
    }
}
