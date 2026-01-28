using HQS.Infrastructure.DTOs;
using HQS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HQS.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using HQS.Hubs;

namespace HQS.Infrastructure.Services
{
    public class HospitalService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<HospitalHub> _hub;

        public HospitalService(ApplicationDbContext db, IHubContext<HospitalHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        public async Task<List<HospitalLookupDto>> GetHospitalsAsync()
        {
            return await _db.Hospitals
                .OrderBy(h => h.Name)
                .Select(h => new HospitalLookupDto
                {
                    Id = h.HospitalId,
                    HospitalName = h.Name
                })
                .ToListAsync();
        }

        // public async Task<List<Hospital>> GetAllHospitalsAsync()
        //     => await _db.Hospitals.AsNoTracking().ToListAsync();

        public async Task<List<Hospital>> GetAllHospitalsAsync()
        {
            var hospitals = new List<Hospital>();
            var query = _db.Hospitals.AsNoTracking().AsAsyncEnumerable();
            try
            {
                await foreach (var hospital in query)
                {
                    // 2. Print details of the successfully mapped item
                    // This lets you see which items are working
                    // Console.WriteLine($"Successfully read Hospital ID: {hospital.HospitalId}, Name: {hospital.Name}");

                    hospitals.Add(hospital);
                }
            }
            catch (Exception ex)
            {
                // 3. Catch the exception exactly where it occurs (the line above this one)
                Console.WriteLine("\n\n---------------------------------");
                Console.WriteLine("!!! FAILED TO MAP A HOSPITAL ROW !!!");
                // The exception will still contain the useful SqlNullValueException details:
                Console.WriteLine($"Error Details: {ex.Message}");
                Console.WriteLine("---------------------------------\n\n");

                // Re-throw the exception if you still want the overall operation to fail
                throw;

                // Or use 'continue;' if you want to skip the bad row and keep processing valid ones
                // continue;
            }


            Console.WriteLine($"Finished reading {hospitals.Count} valid hospitals.");

            return hospitals;
        }

        public async Task AddAsync(Hospital hospital, IBrowserFile? imageFile)
        {
            hospital.HospitalId = Guid.NewGuid();
            hospital.CreatedAt = DateTime.UtcNow;

            if (imageFile != null)
            {
                var ext = Path.GetExtension(imageFile.Name);
                var fileName = $"{hospital.HospitalId}{ext}";
                var savePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "hospital-images",
                    fileName);

                await using var fs = new FileStream(savePath, FileMode.Create);
                await imageFile.OpenReadStream(5_000_000).CopyToAsync(fs);

                hospital.ImagePath = $"/hospital-images/{fileName}";
            }
            else
            {
                hospital.ImagePath = "/hospital-images/hospital-placeholder.png";
            }

            _db.Hospitals.Add(hospital);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid hospitalId)
        {
            var hospital = await _db.Hospitals.FindAsync(hospitalId);
            if (hospital == null) return;

            _db.Hospitals.Remove(hospital);
            await _db.SaveChangesAsync();
        }
        //new
        public async Task<Hospital?> GetByIdAsync(Guid hospitalId)
        {
            return await _db.Hospitals
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HospitalId == hospitalId);
        }

        public async Task<List<string>> GetAllHospitalNamesAsync()
        {
            var existingNames = await _db.Hospitals
                                        .AsNoTracking()
                                        .Select(h => h.Name)
                                        .ToListAsync();
            return existingNames;
        }

        public async Task<bool> CheckNameExistsAsync(string hospitalName)
        {
            if (string.IsNullOrWhiteSpace(hospitalName))
            {
                return false;
            }

            return await _db.Hospitals
                .AsNoTracking() // As we are only checking existence, tracking is not needed
                .AnyAsync(h => h.Name == hospitalName);
        }

        public async Task AddRangeOfNewHospitals(List<Hospital> newHospitals)
        {
            _db.Hospitals.AddRange(newHospitals);
            await _db.SaveChangesAsync();
        }


        //For hospital-rep only
        public async Task UpdateOperationalInfoAsync(UpdateHospitalStatusModel payload)//(Guid hospitalId, int availableBeds, int queueLength, int waitTimeMinutes)
        {
            var hospital = await _db.Hospitals.FindAsync(payload.HospitalId);
            if (hospital == null)
                throw new Exception("Hospital not found");

            if (payload.AvailableBeds > hospital.TotalBeds)
                throw new ApplicationException(
                    $"Available beds ({payload.AvailableBeds}) cannot exceed total beds ({hospital.TotalBeds}).");

            hospital.AvailableBeds = payload.AvailableBeds;
            hospital.QueueLength = payload.QueueLength;
            hospital.WaitTimeMinutes = payload.WaitTimeMinutes;
            
            await _db.SaveChangesAsync();
            
            //signalR code to notify frontend on public website
            await _hub.Clients.All.SendAsync("HospitalDataUpdated", payload);
        }

        public async Task<List<Hospital>> GetPublicHospitalsAsync()
        {
            return await _db.Hospitals
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        public async Task UpdateAsync(Hospital hospital, IBrowserFile? newImage)
        {
            var existing = await _db.Hospitals
                .FirstOrDefaultAsync(h => h.HospitalId == hospital.HospitalId);

            if (existing == null)
                throw new Exception("Hospital not found");

            // Update fields
            existing.Name = hospital.Name;
            existing.Address = hospital.Address;
            existing.PostalCode = hospital.PostalCode;
            existing.TotalBeds = hospital.TotalBeds;
            existing.AvailableBeds = hospital.AvailableBeds;
            existing.QueueLength = hospital.QueueLength;
            existing.WaitTimeMinutes = hospital.WaitTimeMinutes;
            existing.OpenHours = hospital.OpenHours;

            // Optional image replacement
            if (newImage != null)
            {
                var ext = Path.GetExtension(newImage.Name);
                var fileName = $"{existing.HospitalId}{ext}";
                var savePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "hospital-images",
                    fileName);

                await using var fs = new FileStream(savePath, FileMode.Create);
                await newImage.OpenReadStream(5_000_000).CopyToAsync(fs);

                existing.ImagePath = $"/hospital-images/{fileName}";
            }

            existing.ServicesOffered = hospital.ServicesOffered.ToList();

            await _db.SaveChangesAsync();
        }

    }
}
