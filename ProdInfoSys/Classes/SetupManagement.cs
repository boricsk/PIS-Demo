using CalendarManagement;
using MongoDB.Driver;
using ProdInfoSys.DI;
using ProdInfoSys.Models;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    public class SetupManagement
    {
        public static List<MovedWorkDays> LoadTrWorkdays()
        {
            var workday = new MovedWorkDays();
            var movedWorkdayList = new List<MovedWorkDays>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<TrWorkday>(conMgmnt.TrWorkdaysDbName);

            var allDocuments = databaseCollection.Find(FilterDefinition<TrWorkday>.Empty).ToList();
            
            foreach (var document in allDocuments)
            {
                if (document.TransferredWorkdays != null)
                {
                    foreach (var tr in document.TransferredWorkdays)
                    {
                        workday = new MovedWorkDays()
                        {
                            MovedWorkday = tr.FromDay,
                            MovedTo = tr.ToDay,
                        };
                        movedWorkdayList.Add(workday);
                    }
                }
            }
            return movedWorkdayList;
        }

        public static PisSetup LoadSetupData()
        { 
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            return databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();
        }

        public static List<string> GetEmailList()
        {
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.EmailList.ToList();
        }

        public static List<string> GetLeaderEmailList()
        {
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.LeaderEmailList.ToList();
        }

        public static List<string> GetProdMeetingWorkcenterList()
        {
            //var workcenterSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.ProdMeetingWorkcenters.ToList();
        }

        public static void SaveSetupData(PisSetup pisSetup)
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            MongoDbOperations<PisSetup> db = new MongoDbOperations<PisSetup>(databaseCollection);
            _ = db.DeleteAll();
            _ = db.AddNewDocument(pisSetup);
        }

        public static string GetSmtpPassword() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("EmailPw"));
        public static string GetErpUserPassword() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("ErpUserPw"));
        public static string GetErpConString() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("ERPConString"));
    }
}
