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
    /// <summary>
    /// Provides methods for managing and retrieving application setup data, including configuration settings, email
    /// lists, workcenter information, and secure credentials.
    /// </summary>
    /// <remarks>The SetupManagement class offers static methods to access and modify setup-related
    /// information stored in the database or system registry. It includes functionality for retrieving and saving
    /// configuration data, as well as securely handling sensitive information such as passwords and connection strings.
    /// All methods are thread-safe as they do not maintain internal state.</remarks>
    public class SetupManagement
    {
        /// <summary>
        /// Retrieves all moved workday entries from the database.
        /// </summary>
        /// <remarks>This method aggregates all transferred workday records across all documents in the
        /// underlying collection. The returned list may contain multiple entries for the same or different days,
        /// depending on the data stored in the database.</remarks>
        /// <returns>A list of <see cref="MovedWorkDays"/> objects representing all transferred workdays. The list is empty if no
        /// transferred workdays are found.</returns>
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

        /// <summary>
        /// Retrieves the current setup configuration from the database.
        /// </summary>
        /// <returns>A <see cref="PisSetup"/> object containing the setup configuration if found; otherwise, <see
        /// langword="null"/>.</returns>
        public static PisSetup LoadSetupData()
        { 
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            return databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the list of email addresses configured in the system.
        /// </summary>
        /// <returns>A list of strings containing the configured email addresses. The list is empty if no email addresses are
        /// configured.</returns>
        public static List<string> GetEmailList()
        {
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.EmailList.ToList();
        }

        /// <summary>
        /// Retrieves a list of email addresses for all leaders configured in the system.
        /// </summary>
        /// <returns>A list of strings containing the email addresses of leaders. The list is empty if no leader email addresses
        /// are configured.</returns>
        public static List<string> GetLeaderEmailList()
        {
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.LeaderEmailList.ToList();
        }

        /// <summary>
        /// Retrieves a list of workcenter names configured for production meetings.
        /// </summary>
        /// <returns>A list of strings containing the names of workcenters designated for production meetings. The list is empty
        /// if no workcenters are configured.</returns>
        public static List<string> GetProdMeetingWorkcenterList()
        {
            //var workcenterSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            var setupData = databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();

            return setupData.ProdMeetingWorkcenters.ToList();
        }

        /// <summary>
        /// Saves the specified setup data by replacing all existing setup records in the database.
        /// </summary>
        /// <remarks>This method deletes all existing setup records before saving the new setup data. Use
        /// with caution, as any previously stored setup information will be lost.</remarks>
        /// <param name="pisSetup">The setup data to be saved. Cannot be null.</param>
        public static void SaveSetupData(PisSetup pisSetup)
        {            
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            MongoDbOperations<PisSetup> db = new MongoDbOperations<PisSetup>(databaseCollection);
            _ = db.DeleteAll();
            _ = db.AddNewDocument(pisSetup);
        }

        /// <summary>
        /// Retrieves the SMTP password used for email operations from secure storage.
        /// </summary>
        /// <remarks>The password is stored securely and decrypted before being returned. Callers should
        /// ensure that the returned password is handled securely in memory and not logged or exposed.</remarks>
        /// <returns>A string containing the decrypted SMTP password. Returns an empty string if the password is not set.</returns>
        public static string GetSmtpPassword() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("EmailPw"));
        
        /// <summary>
        /// Retrieves the decrypted ERP user password from the system registry.
        /// </summary>
        /// <remarks>The password is stored in the registry in encrypted form and is decrypted before
        /// being returned. Callers should ensure appropriate security measures are taken when handling the returned
        /// password.</remarks>
        /// <returns>A string containing the ERP user password, or null if the password is not set in the registry.</returns>
        public static string GetErpUserPassword() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("ErpUserPw"));
        
        /// <summary>
        /// Retrieves the decrypted ERP connection string from the system registry.
        /// </summary>
        /// <remarks>Use this method to obtain the connection string required to connect to the ERP
        /// system. The connection string is stored in the registry in encrypted form and is decrypted before being
        /// returned.</remarks>
        /// <returns>A string containing the ERP connection string. Returns an empty string if the registry key does not exist or
        /// contains no value.</returns>
        public static string GetErpConString() => DpApiStorage.Decrypt(RegistryManagement.ReadStringRegistryKey("ERPConString"));
    }
}
