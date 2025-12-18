using MongoDB.Bson;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using System.Windows;

namespace ProdInfoSys.DI
{
    /// <summary>
    /// Provides methods and properties for managing MongoDB database connections and accessing collections within the
    /// application.
    /// </summary>
    /// <remarks>This class encapsulates connection management for multiple MongoDB databases used by the
    /// application. It exposes properties for commonly used database names and provides methods to establish
    /// connections, verify connection strings, and retrieve collections. The class is not thread-safe; concurrent
    /// access should be managed externally if used in multi-threaded scenarios.</remarks>
    public class ConnectionManagement : IConnectionManagement
    {
        private IMongoDatabase _database;

        private string _MongoConStringLocal = string.Empty;
        public bool conStatus { get; private set; }
        public string DbName { get; private set; } = "ProductionFollowupDemo";
        public string TrWorkdaysDbName { get; private set; } = "TransferredWorkdaysDemo";
        public string MeetingMemo { get; private set; } = "MeetingMemoDemo";
        public string PisSetupDbName { get; private set; } = "PisSetupDemo";

        public ConnectionManagement()
        {
            conStatus = ConnectToDatabase();
        }

        /// <summary>
        /// Attempts to establish a connection to the configured MongoDB database.
        /// </summary>
        /// <remarks>If the MongoDB connection string is not set in the registry, a default value is
        /// written and used. If the connection attempt fails, an error message is displayed to the user. This method
        /// does not throw exceptions for connection failures; instead, it returns false and shows a message
        /// box.</remarks>
        /// <returns>true if the connection to the MongoDB database is successful; otherwise, false.</returns>
        public bool ConnectToDatabase()
        {
            bool ret = true;
            if (RegistryManagement.ReadStringRegistryKey("MongoConStringLocal") == "")
            {
                RegistryManagement.WriteStringRegistryKey("MongoConStringLocal", "mongodb://192.168.1.190:27017");
            }
            else
            {
                _MongoConStringLocal = RegistryManagement.ReadStringRegistryKey("MongoConStringLocal");
            }

            try
            {
                if (PingConnection(_MongoConStringLocal))
                {
                    var client = new MongoClient(_MongoConStringLocal);
                    _database = client.GetDatabase(DbName);
                }
            }
            catch (Exception e)
            {
                ret = false;
                MessageBox.Show($"Connection Error: {e.Message}", "ConnectionManagement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }

        /// <summary>
        /// Gets a collection from the database with the specified name and document type.
        /// </summary>
        /// <remarks>If the collection does not exist, it is created implicitly when a document is first
        /// inserted. The returned collection instance can be used to perform CRUD operations for documents of type
        /// <typeparamref name="T"/>.</remarks>
        /// <typeparam name="T">The type of the documents stored in the collection.</typeparam>
        /// <param name="collName">The name of the collection to retrieve. Cannot be null or empty.</param>
        /// <returns>An <see cref="IMongoCollection{T}"/> representing the specified collection.</returns>
        public IMongoCollection<T> GetCollection<T>(string collName)
        {
            return _database.GetCollection<T>(collName);
        }

        /// <summary>
        /// Gets the current MongoDB database instance associated with this context.
        /// </summary>
        /// <returns>An <see cref="IMongoDatabase"/> representing the current database, or <see langword="null"/> if no database
        /// is configured.</returns>
        public IMongoDatabase? GetDatabase()
        {
            return _database;
        }

        /// <summary>
        /// Attempts to establish a connection to a MongoDB server using the specified connection string and checks if
        /// the server is reachable.
        /// </summary>
        /// <remarks>This method sends a ping command to the MongoDB server to verify connectivity. It
        /// does not throw exceptions; instead, it returns false if the connection attempt fails for any
        /// reason.</remarks>
        /// <param name="conString">The connection string used to connect to the MongoDB server. Cannot be null or empty.</param>
        /// <returns>true if the connection to the MongoDB server is successful; otherwise, false.</returns>
        public bool PingConnection(string conString)
        {
            try
            {
                var pingClient = new MongoClient(conString);
                var result = pingClient.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
