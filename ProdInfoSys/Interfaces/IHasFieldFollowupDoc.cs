namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// Represents an object that provides a sum of output values for a field follow-up operation.
    /// </summary>
    public interface IHasFieldFollowupDoc
    {
        int OutputSum { get; }
    }
}
