using ProdInfoSys.Models.ErpDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ProdInfoSys.Classes
{
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
