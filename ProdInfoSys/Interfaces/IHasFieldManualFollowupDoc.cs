namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// Represents an object that includes a workday associated with a field manual follow-up document.
    /// </summary>
    public interface IHasFieldManualFollowupDoc
    {
        public DateOnly Workday { get; set; }
    }
}
