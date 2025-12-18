using MongoDB.Bson;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProdInfoSys.DI
{
    /// <summary>
    /// Connection management class
    /// </summary>
    public class ConnectionManagement: IConnectionManagement
    {
        private IMongoDatabase _database;
        
        private string _MongoConStringLocal = string.Empty;
        public bool conStatus { get; private set; }
        public string DbName { get; private set; }  = "ProductionFollowupDemo";
        public string TrWorkdaysDbName { get; private set; }  = "TransferredWorkdaysDemo";
        public string MeetingMemo { get; private set; }  = "MeetingMemoDemo";
        public string PisSetupDbName { get; private set; }  = "PisSetupDemo";
        
        public ConnectionManagement()
        {            
            conStatus = ConnectToDatabase();
        }

        /// <summary>
        /// Database connection
        /// </summary>
        /// <returns></returns>
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
        /// Get connected collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(string collName)
        {
            return _database.GetCollection<T>(collName);
        }

        /// <summary>
        /// Get connected database
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase? GetDatabase()
        {
            return _database;
        }

        /// <summary>
        /// Check validity of connection string
        /// </summary>
        /// <param name="conString"></param>
        /// <returns></returns>
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
