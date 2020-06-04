using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class VkUser : BaseEntity
    {
		public long VkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsClosed { get; set; }
        public bool IsLiked { get; set; }
        public DateTime LikeDate { get; set; }

        public long VkCityId { get; set; }
        public VkCity VkCity { get; set; }

    }
}
