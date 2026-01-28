using HQS.Domain.Enums;

namespace HQS.Domain.Entities;

public class Hospital
{
    public Guid HospitalId { get; set; }

    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string? PostalCode { get; set; }

    // List of services offered
    public List<ServiceType> ServicesOffered { get; set; } = new();

    public string? OpenHours { get; set; }

    public int TotalBeds { get; set; }
    public int AvailableBeds { get; set; }
    public int QueueLength { get; set; }
    public int WaitTimeMinutes { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsWheelchairAccessible { get; set; }

    public double DistanceKm { get; set; }
}
