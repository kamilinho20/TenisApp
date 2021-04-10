using System;
using System.ComponentModel.DataAnnotations;
using TenisApp.Core.Enum;

namespace TenisApp.Core.Model
{
    public class Court
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool HasLighting { get; set; }
        [Required]
        public Surface SurfaceType { get; set; }
        [Required]
        public double PricePerHour { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime OpeningTime { get; set; } = DateTime.UnixEpoch.AddHours(8);
        public DateTime ClosingTime { get; set; } = DateTime.UnixEpoch.AddHours(20);
    }
}