using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.DI
{
    public interface IConnectionManagement
    {
        public bool conStatus { get;  }
        public string DbName { get; }
        public string TrWorkdaysDbName { get; } 
        public string MeetingMemo { get;} 
        public string PisSetupDbName { get; } 

        public bool ConnectToDatabase();
        public IMongoCollection<T> GetCollection<T>(string collName);
        public IMongoDatabase? GetDatabase();
        public bool PingConnection(string conString);
    }
}
