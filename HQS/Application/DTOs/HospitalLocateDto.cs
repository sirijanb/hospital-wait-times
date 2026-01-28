using HQS.Domain.Enums;

namespace HQS.Infrastructure.DTOs
{
    public class HospitalLocateDto //data we are fetching from OverpassAPI
    {
        public Guid HospitalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public List<ServiceType> ServicesOffered { get; set; } = new();
        public string? OpenHours { get; set; } //implement
        public int TotalBeds { get; set; }//NR->remove //Not available in API
        public int AvailableBeds { get; set; }//NR->remove //Not available in API
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ImagePath { get; set; }//Not available in API
        public DateTime LastUpdated { get; set; }
        public bool IsWheelchairAccessible { get; set; }
    }
}
