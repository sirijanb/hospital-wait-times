using HQS.Domain.Entities;
using HQS.Infrastructure.DTOs;

namespace HQS.Infrastructure.Services
{
    public interface IHospitalLocateService
    {
        Task<List<Hospital>> GetNearbyHospitalsAsync(
            double userLat,
            double userLon,
            int radiusInMetres,
            string countryName);
        // Task<List<HospitalLocateDto>> GetNearbyHospitalsAsync(
        // double userLat,
        // double userLon,
        // int radiusInMetres,
        // string countryName);
    }
}