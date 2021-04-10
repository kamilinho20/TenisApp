using System;

namespace TenisApp.Core.Model
{
    public class Term
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Court Court { get; set; }
        public int CourtId { get; set; }
    }
}