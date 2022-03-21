using DrivingTestExplorer.Models;
using Orleans;

namespace DrivingTestExplorer.Services
{
    public class LocationHostedService : BackgroundService
    {
        private readonly IClusterClient client;
        private readonly LocationService locationService;

        public LocationHostedService(IClusterClient client, LocationService locationService)
        {
            this.client = client;
            this.locationService = locationService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await locationService.InitiateLocations();

            while (!stoppingToken.IsCancellationRequested)
            {
                var locations = await client.GetGrain<ILocationManagerGrain>(Guid.Empty).GetAllAsync();

                await Task.WhenAll(locations.Select(async location =>
                {
                    await client.GetGrain<ILocationGrain>(location).UpdateAsync();
                }));
                

                await Task.Delay(3* 60 * 1000, stoppingToken);
            }
        }
    }
}
