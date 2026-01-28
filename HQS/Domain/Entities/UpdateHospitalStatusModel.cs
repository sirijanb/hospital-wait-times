using System.ComponentModel.DataAnnotations;

namespace HQS.Domain.Entities
{
    public class UpdateHospitalStatusModel
    {
        public Guid HospitalId { get; set; }
        [Range(0, 10000)]
        public int AvailableBeds { get; set; }

        [Range(0, 10000)]
        public int QueueLength { get; set; }

        [Range(0, 1440)]
        public int WaitTimeMinutes { get; set; }
    }
}