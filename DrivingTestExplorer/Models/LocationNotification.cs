using DrivingTestExplorer.Models;
using Orleans.Concurrency;

namespace DrivingTestExplorer.Models;

[Immutable, Serializable]
public record class LocationNotification(string Key, LocationModel? Item = null);

