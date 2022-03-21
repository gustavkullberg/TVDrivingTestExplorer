namespace DrivingTestExplorer.Integration.Models
{
    public class GetSearchInformationResponse
    {
        public GetSearchInformationData Data { get; set; }
    }

    public class GetSearchInformationData
    {
        public IEnumerable<LocationModel> Locations { get; set; }
    }

    public class LocationModel
    {
        public IEnumerable<ExaminationCategories> ExaminationCategories { get; set; }
        public Location Location { get; set; }
    }
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ExaminationCategories
    {
        public int Value { get; set; }
    }
}
