using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ApplicationOptions : BaseEntity
    {
        public bool IsCitiesSynchronized { get; set; }
        public uint Offset { get; set; }
    }
}
