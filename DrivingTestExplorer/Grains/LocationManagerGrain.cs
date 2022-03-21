using Orleans;

namespace DrivingTestExplorer.Models
{
    public class LocationManagerGrain : Grain, ILocationManagerGrain
    {
        readonly HashSet<string> _locations = new(); 
        public Task<List<string>> GetAllAsync()
        {
            return Task.FromResult(_locations.ToList());
        }

        public Task RegisterAsync(string key)
        {
            _locations.Add(key);
            return Task.CompletedTask;
        }

        public Task UnregisterAsync(string key)
        {
            _locations.Remove(key);
            return Task.CompletedTask;
        }
    }
}
