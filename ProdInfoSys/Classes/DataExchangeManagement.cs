using ProdInfoSys.Models.ErpDataModels;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides methods for retrieving capacity ledger entries, extended capacity ledger entries, and production plan
    /// data from an ERP system via OData web services.
    /// </summary>
    /// <remarks>This class manages communication with the ERP system using HTTP requests and basic
    /// authentication. It retrieves configuration and credentials from the system registry and other setup sources. All
    /// data retrieval methods are asynchronous and return collections of domain-specific entities. Instances of this
    /// class are not thread-safe.</remarks>
    public class DataExchangeManagement
    {
        private readonly string _serverIp = RegistryManagement.ReadStringRegistryKey("ApiServer");
        private readonly string _erpEnv = RegistryManagement.ReadStringRegistryKey("ErpEnv");
        private readonly string _company = RegistryManagement.ReadStringRegistryKey("ErpCompany");
        private readonly string _user = RegistryManagement.ReadStringRegistryKey("ErpUser");
        private readonly string _passw = SetupManagement.GetErpUserPassword();
        public DataExchangeManagement()
        {

        }

        /// <summary>
        /// Retrieves a collection of capacity ledger entries for the specified work center and posting date.
        /// </summary>
        /// <remarks>This method performs an asynchronous HTTP request to the configured OData service
        /// using basic authentication. The returned collection can be observed for changes. Ensure that the provided
        /// parameters match the expected values in the external system.</remarks>
        /// <param name="workcenter">The identifier of the work center for which to retrieve capacity ledger entries. Cannot be null or empty.</param>
        /// <param name="date">The posting date to filter the ledger entries, formatted as a string. The expected format depends on the
        /// OData service requirements.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an observable collection of
        /// capacity ledger entries matching the specified work center and date. The collection is empty if no entries
        /// are found.</returns>
        public async Task<ObservableCollection<CapacityLedgerEntry>> GetCapacityLedgerEntries(string workcenter, string date)
        {
            ObservableCollection<CapacityLedgerEntry> ret = new ObservableCollection<CapacityLedgerEntry>();

            var url = $"http://{_serverIp}/{_erpEnv}/ODataV4/Company('{_company}')/SEICapacityLedger?$filter=PostingDate eq {date} and Workcenter eq '{workcenter}'";

            var client = new HttpClient();
            var byteArray = Encoding.UTF8.GetBytes($"{_user}:{_passw}");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var json = JsonNode.Parse(result);
                var valueJson = json["value"].ToJsonString();
                var list = JsonSerializer.Deserialize<List<CapacityLedgerEntry>>(valueJson);
                ret = new ObservableCollection<CapacityLedgerEntry>(list);
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }

            return ret;
        }
        //TODO : Szimulációra át kell írni
        /// <summary>
        /// Retrieves a collection of external capacity ledger entries for the specified work center and posting date.
        /// </summary>
        /// <remarks>This method performs an HTTP request to an external OData service using basic
        /// authentication. The returned collection is suitable for data binding in UI scenarios. Network errors or
        /// invalid credentials may result in an empty collection and a message box notification.</remarks>
        /// <param name="workcenter">The identifier of the work center for which to retrieve capacity ledger entries. Cannot be null or empty.</param>
        /// <param name="date">The posting date to filter the ledger entries, formatted as a string. The expected format should match the
        /// OData service requirements.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an observable collection of
        /// external capacity ledger entries matching the specified criteria. The collection will be empty if no entries
        /// are found.</returns>
        public async Task<ObservableCollection<ExtCapacityLedgerEntry>> GetExtCapacityLedgerEntries(string workcenter, string date)
        {
            ObservableCollection<ExtCapacityLedgerEntry> ret = new ObservableCollection<ExtCapacityLedgerEntry>();
            string encodedWorkcenter = HttpUtility.UrlEncode(workcenter);
            var url = $"http://{_serverIp}/{_erpEnv}/ODataV4/Company('{_company}')/SEIExtCapacityLedger?$filter=PostingDate eq {date} and Workcenter eq '{encodedWorkcenter}'";

            var client = new HttpClient();
            var byteArray = Encoding.UTF8.GetBytes($"{_user}:{_passw}");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                //var res2 = JsonSerializer.Deserialize<CapacityLedgerEntry>(result);
                var json = JsonNode.Parse(result);
                var valueJson = json["value"].ToJsonString();
                var list = JsonSerializer.Deserialize<List<ExtCapacityLedgerEntry>>(valueJson);
                ret = new ObservableCollection<ExtCapacityLedgerEntry>(list);
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a list of extended capacity ledger entries with nonzero stop times within the specified posting
        /// date interval.
        /// </summary>
        /// <remarks>The method queries an external OData service and requires valid server configuration
        /// and credentials. Only entries with a stop time greater than zero are returned. Network errors or
        /// authentication failures may result in an empty list and a message box displaying the HTTP status
        /// code.</remarks>
        /// <param name="startDate">The start date of the interval, in a format accepted by the OData service (typically 'yyyy-MM-dd'). Entries
        /// with a posting date greater than or equal to this value are included.</param>
        /// <param name="endDate">The end date of the interval, in a format accepted by the OData service (typically 'yyyy-MM-dd'). Entries
        /// with a posting date less than or equal to this value are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of extended capacity
        /// ledger entries matching the specified date interval and with stop times greater than zero. The list is empty
        /// if no matching entries are found.</returns>
        public async Task<List<ExtCapacityLedgerEntry>> GetExtCapacityLedgerEntriesIntervallStopCodes(string startDate, string endDate)
        {
            List<ExtCapacityLedgerEntry> ret = new List<ExtCapacityLedgerEntry>();

            var url = $"http://{_serverIp}/{_erpEnv}/ODataV4/Company('{_company}')/SEIExtCapacityLedger?$filter=PostingDate ge {startDate} and PostingDate le {endDate} and StopTime gt 0";

            var client = new HttpClient();
            var byteArray = Encoding.UTF8.GetBytes($"{_user}:{_passw}");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                //var res2 = JsonSerializer.Deserialize<CapacityLedgerEntry>(result);
                var json = JsonNode.Parse(result);
                var valueJson = json["value"].ToJsonString();
                var list = JsonSerializer.Deserialize<List<ExtCapacityLedgerEntry>>(valueJson);
                ret = list;
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }

            return ret;
        }

        /// <summary>
        /// Retrieves the production plan entries that match the specified plan name from the ERP system.
        /// </summary>
        /// <remarks>This method performs an HTTP request to the configured ERP OData endpoint using basic
        /// authentication. The returned collection can be observed for changes. If the request fails, an error message
        /// is displayed to the user.</remarks>
        /// <param name="planName">The name of the production plan to retrieve. This value is used to filter the results. Cannot be null or
        /// empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an observable collection of <see
        /// cref="ErpProdPlan"/> objects matching the specified plan name. The collection is empty if no matching
        /// entries are found.</returns>
        public async Task<ObservableCollection<ErpProdPlan>> GetProductionPlan(string planName)
        {
            ObservableCollection<ErpProdPlan> ret = new ObservableCollection<ErpProdPlan>();

            var url = $"http://{_serverIp}/{_erpEnv}/ODataV4/Company('{_company}')/SEIProductionPlan?$filter=Plan_Name eq '{planName}'";

            var client = new HttpClient();
            var byteArray = Encoding.UTF8.GetBytes($"{_user}:{_passw}");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                //var res2 = JsonSerializer.Deserialize<CapacityLedgerEntry>(result);
                var json = JsonNode.Parse(result);
                var valueJson = json["value"].ToJsonString();
                var list = JsonSerializer.Deserialize<List<ErpProdPlan>>(valueJson);
                ret = new ObservableCollection<ErpProdPlan>(list);
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }

            return ret;
        }
    }
}
