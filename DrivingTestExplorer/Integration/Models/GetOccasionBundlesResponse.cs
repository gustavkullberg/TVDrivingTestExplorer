namespace DrivingTestExplorer.Integration.Models
{
    public class GetOccasionBundlesResponse
    {
        public OccationBundlesData Data { get; set; }

    }
    public class OccationBundlesData
    {
        public IEnumerable<Bundle> Bundles { get; set; }
    }

    public class Bundle
    {
        public IEnumerable<Occasion> Occasions { get; set; }
        public string Cost { get; set; }
    }

    public class Occasion
    {
        public Duration Duration { get; set; }
        public string LocationName { get; set; }
        public bool IsLateCancellation { get; set; }
    }

    public class Duration
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
