using Orleans;

namespace DrivingTestExplorer.Models
{
    public interface ILocationManagerGrain : IGrainWithGuidKey
    {
        Task RegisterAsync(string key);
        Task UnregisterAsync(string key);
        Task<List<string>> GetAllAsync();
    }
}
