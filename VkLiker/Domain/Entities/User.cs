using System;
using System.Collections.Generic;
using System.Text;
using Database;

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

        public long CityId { get; set; }
        public City City { get; set; }

    }
}
