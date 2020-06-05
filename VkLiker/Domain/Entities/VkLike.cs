using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class VkLike : BaseEntity
    {
        public long CurrentUserId { get; set; }
        public User CurrentUser { get; set; }
        public DateTime Date { get; set; }
    }
}
