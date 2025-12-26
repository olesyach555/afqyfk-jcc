using System;

namespace ReservesOfRussia.DAL.Models
{
    public class Reserve
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Area { get; set; }
        public DateTime? FoundationDate { get; set; }
        public int RegionId { get; set; }

        // Navigation property
        public virtual Region Region { get; set; }
    }
}
