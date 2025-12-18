using MongoDB.Driver;

namespace ProdInfoSys.DI
{
    /// <summary>
    /// Defines methods and properties for managing connections to MongoDB databases, including connection status,
    /// database names, and access to collections.
    /// </summary>
    /// <remarks>Implementations of this interface provide functionality to establish, monitor, and interact
    /// with MongoDB database connections. This interface is intended for use in scenarios where multiple databases or
    /// collections may be accessed, and where connection status and configuration details are required.</remarks>
    public interface IConnectionManagement
    {
        public bool conStatus { get; }
        public string DbName { get; }
        public string TrWorkdaysDbName { get; }
        public string MeetingMemo { get; }
        public string PisSetupDbName { get; }

        public bool ConnectToDatabase();
        public IMongoCollection<T> GetCollection<T>(string collName);
        public IMongoDatabase? GetDatabase();
        public bool PingConnection(string conString);
    }
}
