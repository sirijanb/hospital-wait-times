using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HQS.Domain.Entities;
using HQS.Infrastructure.Data;
using HQS.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HQS.Infrastructure.Services
{
    public class HospitalLocateService : IHospitalLocateService
    {
        /*private readonly ApplicationDbContext _context;

        public HospitalLocateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HospitalLocateDto>> GetNearbyHospitalsAsync(
            double userLat,
            double userLon,
            int radiusKm = 10)
        {
            var hospitals = await _context.Hospitals
                .Where(h => h.Latitude != null && h.Longitude != null)
                .Select(h => new HospitalLocateDto
                {
                    HospitalId = h.HospitalId,
                    Name = h.Name,
                    Address = h.Address,
                    ImagePath = h.ImagePath,
                    Latitude = h.Latitude!.Value,
                    Longitude = h.Longitude!.Value,
                    TotalBeds = h.TotalBeds,
                    AvailableBeds = h.AvailableBeds,
                    ServicesOffered = h.ServicesOffered
                })
                .ToListAsync();

            // Optional: distance filtering later
            return hospitals;
        }*/ //from our own db

        private readonly HttpClient _httpClient;
        private List<HospitalLocateDto> hospitalsFound;

        private readonly HospitalService _hospitalService;
        List<Hospital> hospitalsToAdd = new List<Hospital>();

        public HospitalLocateService(HttpClient httpClient, HospitalService hospitalService)
        {
            _httpClient = httpClient;
            hospitalsFound = new List<HospitalLocateDto>();
            _hospitalService = hospitalService;
        }

        // public async Task<List<HospitalLocateDto>> 
        public async Task<List<Hospital>> GetNearbyHospitalsAsync(double lat, double lon, int radiusInMeters, string country)
        {
            // var query = $@"
            //             [out:json][timeout:45];
            //             (
            //             nwr[""amenity""~""hospital""](around:{radiusInMeters},{lat},{lon});
            //             nwr[""healthcare""~""hospital""](around:{radiusInMeters},{lat},{lon});
            //             );
            //             out center tags;";

            // var query2 = @"
            //             [out:json][timeout:120];
            //             area[""name""=""Canada""][""boundary""=""administrative""][""admin_level""=""2""]->.country_area;
            //             (
            //             nwr[""amenity""~""hospital""](area.country_area)->.all_hospitals;
            //             nwr[""healthcare""~""hospital""](area.country_area)->.all_hospitals;
            //             );
            //             nwr.all_hospitals(around:{radiusInMeters},{lat},{lon});
            //             out center tags;
            //         ";

            var query = $@"
                    [out:json][timeout:120];
                    area[""name""=""{country}""][""boundary""=""administrative""][""admin_level""=""2""]->.country_area;
                    (
                    nwr[""amenity""~""hospital""](area.country_area)->.all_hospitals;
                    nwr[""healthcare""~""hospital""](area.country_area)->.all_hospitals;
                    );
                    // Dynamic parameters inserted here:
                    nwr.all_hospitals(around:{radiusInMeters}, {lat}, {lon});
                    out center tags;
                ";

            Console.WriteLine($">>>> Q: {query}");
            // FIX: Use FormUrlEncodedContent instead of StringContent
            var content = new FormUrlEncodedContent
            (
                new[]
                {
                    new KeyValuePair<string, string>("data", query)
                }
            );
            // Console.WriteLine($"~~~~>> Query: {query}");
            try
            {
                var response = await _httpClient.PostAsync
                (
                    "https://overpass-api.de/api/interpreter",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"~~~~>> GOT ERROR: {response.StatusCode}): {errorMsg}");
                    return new List<Hospital>();//TEMP WorkAround
                    throw new Exception($"~~~~>> Overpass API Error ({response.StatusCode}): {errorMsg})");
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"~~~~>> GOT Json Data");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<OverpassResponse>(json, options);
                if (data != null)
                {
                    hospitalsToAdd.Clear();
                    hospitalsFound.Clear();

                    var existingNames = await _hospitalService.GetAllHospitalNamesAsync();

                    var existingNameSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
                    
                    Console.WriteLine($">>> existing count: {existingNameSet.Count}");

                    await ParseDataAndAddNewHospitalsToDB(data, existingNameSet);
                    
                    var _list = await _hospitalService.GetAllHospitalsAsync();
                    return _list;
                }
                else
                {
                    throw new Exception("Unable to fetch json data!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return _hospitalService.GetAllHospitalsAsync().Result;
            }
        }

        // List<HospitalLocateDto> 
        async Task ParseDataAndAddNewHospitalsToDB(OverpassResponse data, HashSet<string> existingHospitalNames)
        {
            foreach (var item in data.Elements)
            {
                HospitalLocateDto hospital = new HospitalLocateDto();
                if (item.Tags != null && item.Tags.TryGetValue("name", out var _name))
                {
                    if (string.IsNullOrWhiteSpace(_name))
                    {
                        Console.WriteLine(">>> 1-hospital doesn't have a name!!!");
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(_name) && existingHospitalNames.Contains(_name))
                    {
                        Console.WriteLine(">>> hospital EXISTS with same name!!!");
                        continue; //hospital data exists in our db, so no need to add again
                    }
                    Console.WriteLine($">>> hospital NEW {_name}");
                    hospital.Name = _name;
                }
                else
                {
                    Console.WriteLine(">>> 2-hospital doesn't have a name!!!");
                    continue;
                }

                hospital.HospitalId = Guid.NewGuid();

                if (item.Tags != null)
                {
                    StringBuilder addressBuilder = new StringBuilder();

                    if (item.Tags.TryGetValue("addr:housenumber", out var _houseNo))
                    {
                        // addressBuilder.AppendJoin(",",_houseNo);
                    }
                    if (item.Tags.TryGetValue("addr:street", out var _street))
                    {
                        // addressBuilder.AppendJoin(",", _street);
                    }
                    if (item.Tags.TryGetValue("addr:city", out var _city))
                    {
                        // addressBuilder.AppendJoin(",", _city);
                    }
                    if (item.Tags.TryGetValue("addr:province", out var _province))
                    {
                        // addressBuilder.AppendJoin(", ", _province);
                    }

                    if (item.Tags.TryGetValue("addr:postcode", out var _postalCode))
                    {
                        hospital.PostalCode = _postalCode;
                    }
                    else
                    {
                        hospital.PostalCode = null;
                    }

                    var parts = new[] { _houseNo, _street, _city, _province };
                    addressBuilder.AppendJoin(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));

                    hospital.ServicesOffered.Add(Domain.Enums.ServiceType.General);

                    if (item.Tags.TryGetValue("emergency", out var _emergency))
                    {
                        if (_emergency.Equals("yes", StringComparison.OrdinalIgnoreCase))
                        {
                            hospital.ServicesOffered.Add(Domain.Enums.ServiceType.Emergency);
                        }
                    }

                    if (item.Tags.TryGetValue("opening_hours", out var _openHours))
                    {
                        hospital.OpenHours = _openHours;
                    }
                    else
                    {
                        hospital.OpenHours = null;
                    }

                    if (item.Tags.TryGetValue("website", out var _website))
                    {
                        hospital.Website = _website;
                    }
                    else
                    {
                        hospital.Website = null;
                    }

                    if (item.Tags.TryGetValue("phone", out var _phone))
                    {
                        hospital.Phone = _phone;
                    }
                    else
                    {
                        hospital.Phone = null;
                    }

                    if (item.Tags.TryGetValue("wheelchair", out var _wc))
                    {
                        hospital.IsWheelchairAccessible = true;
                    }
                    else
                    {
                        hospital.IsWheelchairAccessible = false;
                    }

                    hospital.Address = addressBuilder.ToString();
                }

                if (item.Type.Equals("node"))
                {
                    hospital.Latitude = item.Lat;
                    hospital.Longitude = item.Lon;
                }
                else if (item.Type.Equals("way") || item.Type.Equals("relation"))
                {
                    hospital.Latitude = item.Center.Lat;
                    hospital.Longitude = item.Center.Lon;
                }

                hospitalsFound.Add(hospital);
            }

            Console.WriteLine($">>> NEW hospitalsFound: {hospitalsFound.Count}");
            await AddNewHospitalsToDB(hospitalsFound);
            // return hospitalsFound;
        }

        async Task AddNewHospitalsToDB(List<HospitalLocateDto> hospitals)
        {
            Random _random = new Random();
            foreach (var h in hospitals)
            {
                Hospital newHospital = new Hospital();
                newHospital.HospitalId = h.HospitalId;
                newHospital.Name = h.Name;
                newHospital.Address = h.Address;
                newHospital.PostalCode = h.PostalCode;
                newHospital.ServicesOffered = h.ServicesOffered;
                newHospital.OpenHours = h.OpenHours;
                newHospital.TotalBeds = _random.Next(30, 151);
                newHospital.AvailableBeds = _random.Next(1, newHospital.TotalBeds + 1);
                newHospital.QueueLength = _random.Next(11, 38);
                newHospital.WaitTimeMinutes = newHospital.QueueLength * _random.Next(20, 51);
                newHospital.Phone = h.Phone;
                newHospital.Website = h.Website;
                newHospital.Latitude = h.Latitude;
                newHospital.Longitude = h.Longitude;
                newHospital.ImagePath = "/hospital-images/hospital-placeholder.png";
                hospitalsToAdd.Add(newHospital);
            }
            Console.WriteLine($">>> NEW hospitals ready to ADD {hospitalsToAdd.Count}");

            if (hospitalsToAdd.Any())
            {
                await _hospitalService.AddRangeOfNewHospitals(hospitalsToAdd);
            }
        }
    }
}