namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// Defines properties for an object that includes information about a workday, shift number, and shift length for a
    /// health care follow-up document.
    /// </summary>
    public interface IHasFieldHCFollowupDoc
    {
        public DateOnly Workday { get; set; }
        public int ShiftNum { get; set; }
        public decimal ShiftLen { get; set; }

    }
}
