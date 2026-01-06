using System.ComponentModel.DataAnnotations.Schema;

namespace HQS.Domain.Entities
{

    public class HospitalRep
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ContactNumber { get; set; } = default!;
        // FK → Hospital
        public Guid HospitalId { get; set; }
        public Hospital Hospital { get; set; } = null!;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}