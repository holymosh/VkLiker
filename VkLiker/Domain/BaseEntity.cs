using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public abstract class BaseEntity
    {
        [ForeignKey("Id")]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
