using System;
using TenisApp.Core.Enum;

namespace TenisApp.Core.Model
{
    public class Reservation
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int TermId { get; set; }
        public Term Term { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}