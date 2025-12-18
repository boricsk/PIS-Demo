namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// Represents an object that includes information about a workday and its associated available operating hours for
    /// a field machine follow-up document.
    /// </summary>
    public interface IHasFieldMachineFollowupDoc
    {
        public DateOnly Workday { get; set; }
        public double AvailOperatingHour { get; set; }
    }
}
