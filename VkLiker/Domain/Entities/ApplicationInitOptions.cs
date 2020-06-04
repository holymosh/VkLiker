using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ApplicationInitOptions : BaseEntity
    {
        public bool IsCitiesSynchronized { get; set; }
    }
}
