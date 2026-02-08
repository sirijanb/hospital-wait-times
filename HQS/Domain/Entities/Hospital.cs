using System.ComponentModel.DataAnnotations;
using HQS.Domain.Enums;

namespace HQS.Domain.Entities;

public class Hospital
{
    public Guid HospitalId { get; set; }

    [Required(ErrorMessage = "Hospital Name is required")]
    [StringLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = default!;

    [Required(ErrorMessage = "Postal Code is required")]
    [RegularExpression(@"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z] \d[ABCEGHJ-NPRSTV-Z]\d$", 
    // [RegularExpression(@"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z]\d[ABCEGHJ-NPRSTV-Z]\d$",
    ErrorMessage = "Invalid Canadian Postal Code (Format: A1A 1A1)")]
    public string? PostalCode { get; set; }


    [MinLength(1, ErrorMessage = "Please select at least one service")]
    public List<ServiceType> ServicesOffered { get; set; } = new();

    [Required(ErrorMessage = "Opening hours are required")]
    [RegularExpression(@"^(24 x 7|([1-9]|1[0-2])(am|pm) - ([1-9]|1[0-2])(am|pm))$",
        ErrorMessage = "Format must be '24 x 7' or '3am - 9pm'")]
    public string OpenHours { get; set; } = default!;

    [Range(0, 10000, ErrorMessage = "Total Beds must be a positive number")]
    public int TotalBeds { get; set; }
    public int AvailableBeds { get; set; }
    public int QueueLength { get; set; }
    public int WaitTimeMinutes { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    // [RegularExpression(@"^(\+\d{1,3}[- ]?)?\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
    // ErrorMessage = "Phone number must be of type (e.g., 555-555-5555, +1-(555)-555-5555)")]
    [RegularExpression(@"^[0-9]{10}$", 
    ErrorMessage = "Phone must be exactly 10 digits with no spaces or symbols (e.g., 5142888201)")]
    public string? Phone { get; set; }

    // [Url(ErrorMessage = "Invalid Website URL (must include http:// or https://)")]
    public string? Website { get; set; }

    [Required(ErrorMessage = "Latitude is required")]
    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Longitude is required")]
    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180")]
    public double? Longitude { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsWheelchairAccessible { get; set; }

    public double DistanceKm { get; set; }
}
