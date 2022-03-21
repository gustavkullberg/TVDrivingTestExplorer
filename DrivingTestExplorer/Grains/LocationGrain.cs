using DrivingTestExplorer.Services;
using DrivingTestExplorer.Integration;
using DrivingTestExplorer.Integration.Models;
using Orleans;

namespace DrivingTestExplorer.Models;

public class LocationGrain : Grain, ILocationGrain
{
    private readonly ILogger<LocationGrain> _logger;
    private readonly DistanceService _distanceService;
    private readonly TrafikverketApiService _trafikverketApiService;
    private LocationModel _data;
    private string GrainType => nameof(LocationGrain);
    private string GrainKey => this.GetPrimaryKeyString();

    public LocationGrain(ILogger<LocationGrain> logger, TrafikverketApiService trafikverketApiService, DistanceService distanceService)
    {
        _logger = logger;
        _distanceService = distanceService;
        _trafikverketApiService = trafikverketApiService;
    }
    public override async Task OnActivateAsync()
    {
        _logger.LogInformation($"Grain {GrainKey} just appeared!");
        await base.OnActivateAsync();
    }

    public Task<LocationModel> GetAsync()
    {
        return Task.FromResult(_data);
    }

    public async Task UpdateAsync()
    {
        var topSlot = new Slot(DateTime.Parse("3000-01-01T00:00:00.000Z"), null, false);
        var location = await _trafikverketApiService.GetLocation(GrainKey);
        var bundles = await _trafikverketApiService.GetBundlesForLocation(location.Id);

        if (bundles is null)
        {
            _logger.LogCritical($"bundles was null for {GrainKey}");
            return;
        }
        var slots = new List<Slot>();
        foreach (var bundle in bundles)
        {
            var occasion = bundle.Occasions.FirstOrDefault();
            if (occasion.Duration.Start < topSlot.Date)
            {
                topSlot = new Slot(occasion.Duration.Start, bundle.Cost, occasion.IsLateCancellation);
            }
            slots.Add(new Slot(occasion.Duration.Start, bundle.Cost, occasion.IsLateCancellation));
        }
        var stockholm = new Location { Coordinates = new Coordinates { Latitude = 59.3, Longitude = 18 } };

        var distance = _distanceService.CalculateDistance(stockholm, location);
        _data = new LocationModel(location, topSlot, slots, DateTime.Now, distance);
        await GrainFactory.GetGrain<ILocationManagerGrain>(Guid.Empty).RegisterAsync(location.Name);


        GetStreamProvider("SMS").GetStream<LocationNotification>(Guid.Empty, nameof(ILocationGrain))
            .OnNextAsync(new LocationNotification(GrainKey, _data))
            .Ignore();
    }
}
