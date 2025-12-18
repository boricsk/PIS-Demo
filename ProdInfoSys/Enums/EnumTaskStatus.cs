namespace ProdInfoSys.Enums
{
    /// <summary>
    /// Represents the lifecycle state of a task within the application.
    /// </summary>
    public enum EnumTaskStatus
    {
        /// <summary>
        /// The task is open and may be in progress or awaiting action.
        /// </summary>
        Nyitott,

        /// <summary>
        /// The task is closed and no further action is required.
        /// </summary>
        Lezárt,

        /// <summary>
        /// No status has been assigned to the task.
        /// </summary>
        Nincs
    }
}
