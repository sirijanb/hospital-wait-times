using HQS.Infrastructure.DTOs;
using HQS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HQS.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace HQS.Infrastructure.Services
{

    public class HospitalService
    {
        private readonly ApplicationDbContext _db;

        public HospitalService(ApplicationDbContext db)
        {
            _db = db;
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

        public async Task<List<Hospital>> GetAllHospitalsAsync()
            => await _db.Hospitals.AsNoTracking().ToListAsync();

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
                hospital.ImagePath = "/hospital-images/default.png";
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

        //For hospital-rep only
        public async Task UpdateOperationalInfoAsync(
            Guid hospitalId,
            int availableBeds,
            int queueLength,
            int waitTimeMinutes)
        {
            var hospital = await _db.Hospitals.FindAsync(hospitalId);
            if (hospital == null)
                throw new Exception("Hospital not found");

            if (availableBeds > hospital.TotalBeds)
                throw new ApplicationException(
                    $"Available beds ({availableBeds}) cannot exceed total beds ({hospital.TotalBeds}).");

            hospital.AvailableBeds = availableBeds;
            hospital.QueueLength = queueLength;
            hospital.WaitTimeMinutes = waitTimeMinutes;

            await _db.SaveChangesAsync();
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

            await _db.SaveChangesAsync();
        }

    }
}
