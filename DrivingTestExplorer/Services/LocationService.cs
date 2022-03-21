using DrivingTestExplorer.Models;
using DrivingTestExplorer.Integration;
using Orleans;
using Orleans.Streams;

namespace DrivingTestExplorer.Services
{
    public class LocationService
    {
        private readonly IClusterClient _clusterClient;
        private readonly TrafikverketApiService _trafikverketApiService;
        private readonly DistanceService _distanceService;
        private readonly ILogger<LocationService> _logger;
        public List<Models.LocationModel> _candidates = new();

        public LocationService(IClusterClient clusterClient, TrafikverketApiService trafikverketApiService, DistanceService distanceService, ILogger<LocationService> logger)
        {
            _clusterClient = clusterClient;
            _trafikverketApiService = trafikverketApiService;
            _distanceService = distanceService;
            _logger = logger;
        }
        public async Task InitiateLocations()
        {
            var locations = await _trafikverketApiService.GetLocations();

            await Task.WhenAll(locations.Select(async location =>
            {
                await _clusterClient.GetGrain<ILocationGrain>(location.Name).UpdateAsync();
            }));
        }

        public async Task<List<Models.LocationModel>> Get()
        {
            var locationNames = await _clusterClient.GetGrain<ILocationManagerGrain>(Guid.Empty).GetAllAsync();

            return (await Task.WhenAll(locationNames.Select(async name =>
            {
                var location = _clusterClient.GetGrain<ILocationGrain>(name);
                return await location.GetAsync();
            })))
            .OrderBy(x => x.TopSlot.Date)
            .ToList();
        }

        public async Task<Models.LocationModel> GetSingle(string name)
        {
            var location = _clusterClient.GetGrain<ILocationGrain>(name);
            return await location.GetAsync();
        }

        public Task<StreamSubscriptionHandle<LocationNotification>> SubscribeAsync(Guid ownerKey, Func<LocationNotification, Task> action) =>
            _clusterClient.GetStreamProvider("SMS")
                .GetStream<LocationNotification>(ownerKey, nameof(ILocationGrain))
                .SubscribeAsync(new LocationObserver(_logger, action));

        private class LocationObserver : IAsyncObserver<LocationNotification>
        {
            private readonly ILogger<LocationService> _logger;
            private readonly Func<LocationNotification, Task> _onNext;

            public LocationObserver(
                ILogger<LocationService> logger,
                Func<LocationNotification, Task> action)
            {
                _logger = logger;
                _onNext = action;
            }

            public Task OnCompletedAsync() => Task.CompletedTask;

            public Task OnErrorAsync(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Task.CompletedTask;
            }

            public Task OnNextAsync(
                LocationNotification item,
                StreamSequenceToken? token = null) =>
                _onNext(item);
        }
    }
}
