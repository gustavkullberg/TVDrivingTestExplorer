using DrivingTestExplorer.Integration.Models;

namespace DrivingTestExplorer.Models;

public record LocationModel(Location Location, Slot TopSlot, IEnumerable<Slot> Slots, DateTime UpdatedAt, double Distance)
{
    public Slot TopSlot { get; set; } = TopSlot;
    public DateTime UpdatedAt { get; set; } = UpdatedAt;
}

public record Slot(DateTime Date, string Price, bool IsLateCancellation);
