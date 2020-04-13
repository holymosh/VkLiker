using System;
using System.Collections.Generic;
using System.Text;
using Database;

namespace Domain.Entities
{
    public class VkLike : BaseEntity
    {
        public long PreviousId { get; set; }
        public VkUser Previous { get; set; }

        public long CurrentUserId { get; set; }
        public VkUser CurrentUser { get; set; }
    }
}
