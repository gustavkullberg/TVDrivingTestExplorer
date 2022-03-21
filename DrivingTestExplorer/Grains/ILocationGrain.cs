using Orleans;
namespace DrivingTestExplorer.Models;

public interface ILocationGrain : IGrainWithStringKey
{
    Task UpdateAsync();
    Task<LocationModel> GetAsync();
}
