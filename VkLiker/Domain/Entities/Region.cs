using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Region : BaseEntity
    {
        public string Title { get; set; }
        public long SourceId { get; set; }
    }
}
