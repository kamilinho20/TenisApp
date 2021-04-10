using System.Collections.Generic;

namespace TenisApp.Core.Model
{
    public class AppUser
    {
        public int Id { get; set; }
        public int? ReliefId { get; set; }
        public Relief Relief { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}