namespace ProdInfoSys.Enums
{
    /// <summary>
    /// Defines the supported connection string types used by the application.
    /// </summary>
    public enum EnumConStringType
    {
        /// <summary>
        /// Connection string used for the ERP system (e.g., SQL/Business Central).
        /// </summary>
        ERP,

        /// <summary>
        /// Connection string used for a MongoDB data source.
        /// </summary>
        Mongo
    }
}
