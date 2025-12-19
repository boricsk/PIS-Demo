using MongoDB.Bson;
using MongoDB.Driver;
using Projector.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projector.Models
{
    /// <summary>
    /// Provides methods and properties for managing connections to MongoDB databases, including establishing
    /// connections and accessing collections.
    /// </summary>
    /// <remarks>This class encapsulates connection management for multiple MongoDB databases used within the
    /// application. It exposes properties for commonly used database names and provides methods to connect to the
    /// database and retrieve collections. The connection status is available through the conStatus property. This class
    /// is not thread-safe.</remarks>
    public class ConnectionManagement
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
        /// Attempts to establish a connection to the MongoDB database using the configured connection string.
        /// </summary>
        /// <remarks>If the connection string is not set in the registry, a default value of
        /// "mongodb://localhost:27017" is used. If the connection attempt fails, an error message is displayed to the
        /// user.</remarks>
        /// <returns>true if the connection to the database is successful; otherwise, false.</returns>
        public bool ConnectToDatabase()
        {
            bool ret = true;
            if (RegistryManagement.ReadStringRegistryKey("MongoConStringLocal") == "")
            {
                RegistryManagement.WriteStringRegistryKey("MongoConStringLocal", "mongodb://localhost:27017");
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
        /// Gets a collection with the specified name from the database.
        /// </summary>
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
        /// Attempts to establish a connection to a MongoDB server using the specified connection string and performs a
        /// ping operation to verify connectivity.
        /// </summary>
        /// <remarks>This method can be used to check whether a MongoDB server is reachable and accepting
        /// connections. It does not throw exceptions for connection failures; instead, it returns false if the
        /// connection or ping operation fails.</remarks>
        /// <param name="conString">The connection string used to connect to the MongoDB server. Cannot be null or empty.</param>
        /// <returns>true if the connection to the MongoDB server is successful and the ping command completes; otherwise, false.</returns>
        public static bool PingConnection(string conString)
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
