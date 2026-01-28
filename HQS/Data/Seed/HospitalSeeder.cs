using HQS.Domain.Entities;
using HQS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HQS.Infrastructure.Data.Seed;

public static class HospitalSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        //if the "Hospitals" table is not empty -> return
        if (await db.Hospitals.AnyAsync())
            return;

        var hospitals = new List<Hospital>
        {
            new()
            {
                HospitalId = Guid.NewGuid(),
                Name = "City General Hospital",
                Address = "123 Main Street",
                PostalCode = "M1A 2A2",
                ServicesOffered = new List<ServiceType>
                {
                    ServiceType.Emergency,
                    ServiceType.UPCC
                },
                OpenHours = "24x7",
                TotalBeds = 120,
                AvailableBeds = 100,      // default = total beds
                QueueLength = 0,
                WaitTimeMinutes = 0,

                Latitude = null,          // filled later via geocoding
                Longitude = null,
                ImagePath = "/hospital-images/5598f734-391e-44c7-afc5-d05f92e75e61.jpg",
            },
            new()
            {
                HospitalId = Guid.NewGuid(),
                Name = "Downtown Health Centre",
                Address = "456 Queen Street",
                PostalCode = "M2B 2B2",
                ServicesOffered = new List<ServiceType>
                {
                    ServiceType.UPCC
                },
                OpenHours = "08:00 - 22:00",
                TotalBeds = 60,
                AvailableBeds = 50,      // default = total beds
                QueueLength = 0,
                WaitTimeMinutes = 0,

                Latitude = null,          // filled later via geocoding
                Longitude = null,
                ImagePath = "/hospital-images/012672b4-5738-440f-99ef-05f552e5b741.jpg",
            },
            new()
            {
                HospitalId = Guid.NewGuid(),
                Name = "General Health Centre",
                Address = "11 Bay Street",
                PostalCode = "L2B 2L2",
                ServicesOffered = new List<ServiceType>
                {
                    ServiceType.UPCC
                },
                OpenHours = "10:00 - 22:00",
                TotalBeds = 30,
                AvailableBeds = 20,      // default = total beds
                QueueLength = 0,
                WaitTimeMinutes = 0,

                Latitude = null,          // filled later via geocoding
                Longitude = null,
                ImagePath = "/hospital-images/ae0cfb35-98ea-4679-b86c-8821e548e9a3.png",
            }
        };
        
        db.Hospitals.AddRange(hospitals);//EFFICIENT WAY
        
        await db.SaveChangesAsync();
        
    }
}

/*
// EFFICIENT: Use this approach
List<Hospital> hospitalEntities = hospitalsList.Select(dto => Mapper.Map<Hospital>(dto)).ToList();

// 1. Tell the DbContext about ALL new entities at once
_db.Hospitals.AddRange(hospitalEntities);

// 2. Execute a single, efficient database transaction for all 100 records
await _db.SaveChangesAsync(); 

*/
