using DrivingTestExplorer.Integration.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace DrivingTestExplorer.Integration
{

    public class TrafikverketApiService
    {
        const string _ssn = "XXXXXXXXXX";
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TrafikverketApiService> _logger;

        public TrafikverketApiService(IMemoryCache memoryCache, ILogger<TrafikverketApiService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<IEnumerable<Location>> GetLocations()
        {
            return await _memoryCache.GetOrCreateAsync("locations", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(4);
                using var httpClient = new HttpClient();
                var serializedRequest = JsonSerializer.Serialize(new BookingSession(_ssn), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var result = await httpClient.PostAsync("https://fp.trafikverket.se/Boka/search-information", new StringContent(serializedRequest, Encoding.UTF8, "application/json"));
                var content = await result.Content.ReadAsStringAsync();
                var response = await result.Content.ReadFromJsonAsync<GetSearchInformationResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return response.Data.Locations.Select(x => new Location { Name = x.Location.Name, Id = x.Location.Id, Coordinates = x.Location.Coordinates });

            });
        }

        public async Task<Location?> GetLocation(int locationId)
        {
            var locations = await GetLocations();
            return locations.FirstOrDefault(l => l.Id == locationId);
        }

        public async Task<Location?> GetLocation(string locationName)
        {
            var locations = await GetLocations();
            return locations.FirstOrDefault(l => l.Name == locationName);
        }

        public async Task<IEnumerable<Bundle>> GetBundlesForLocation(int locationId)
        {
            using var httpClient = new HttpClient();
            var request = new GetOccasionBundlesRequest
            {
                OccasionBundleQuery = new OccasionBundleQuery
                {
                    StartDate = DateTime.Parse("1970-01-01T00:00:00.000Z").ToString(),
                    NearbyLocationIds = new List<int>(),
                    LocationId = locationId
                },
                BookingSession = new BookingSession(_ssn)
            };

            var serializedRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            try
            {
                var result = await httpClient.PostAsync("https://fp.trafikverket.se/Boka/occasion-bundles", new StringContent(serializedRequest, Encoding.UTF8, "application/json"));

                var content = await result.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<GetOccasionBundlesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var bundles = response.Data.Bundles;
                return bundles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<Bundle>() ;
            }
        }
    }
}
