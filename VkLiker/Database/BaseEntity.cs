using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public abstract class BaseEntity
    {
        [ForeignKey("Id")]
        public long Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
