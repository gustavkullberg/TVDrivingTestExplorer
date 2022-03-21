namespace DrivingTestExplorer.Integration.Models
{
    public class GetOccasionBundlesRequest
    {
        public BookingSession BookingSession { get; set; }
        public OccasionBundleQuery OccasionBundleQuery { get; set; }

    }

    public class OccasionBundleQuery
    {
        public string StartDate { get; set; }
        public int SearchedMonths => 0;
        public int LocationId { get; set; }
        public IEnumerable<int> NearbyLocationIds { get; set; }
        public int VehicleTypeId => 4;
        public int TachographTypeId => 1;
        public int OccasionChoiceId => 1;
        public int ExaminationTypeId => 12;
    }

    public class BookingSession
    {
        public string SocialSecurityNumber { get; set; }
        public int LicenceId => 5;
        public int BookingModeId => 0;
        public bool IgnoreDebt => false;

        public BookingSession(string ssn)
        {
            SocialSecurityNumber = ssn;
        }
    }
}
