using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User : ExternalEntity
    {
		public long VkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsClosed { get; set; }
        public bool IsLiked { get; set; }
        public DateTime LikeDate { get; set; }

        public long VkCityId { get; set; }
        public RegionPart RegionPart { get; set; }
        public long? PreviousId { get; set; }
        public User Previous { get; set; }

        public ICollection<User> Friends { get; set; }

    }
}
