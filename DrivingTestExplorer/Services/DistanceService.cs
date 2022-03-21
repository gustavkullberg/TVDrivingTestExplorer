using DrivingTestExplorer.Integration.Models;
using GeoCoordinatePortable;

namespace DrivingTestExplorer.Services
{
    public class DistanceService
    {
        public int CalculateDistance(Location l1, Location l2)
        {
            var c1 = new GeoCoordinate(l1.Coordinates.Latitude, l1.Coordinates.Longitude);
            var c2 = new GeoCoordinate(l2.Coordinates.Latitude, l2.Coordinates.Longitude);

            return (int)(c1.GetDistanceTo(c2) / 1000);
        }
    }
}
