using ProdInfoSys.Interfaces;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.DI
{
    public interface IUserControlFunctions
    {
        public int GetComulatedOut<T>(IEnumerable<T> followupDocument) where T : IHasFieldFollowupDoc;

        public IEnumerable<T> AddExtraWorkday<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldHCFollowupDoc, new();
        public IEnumerable<T> AddExtraWorkdayMachine<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldMachineFollowupDoc, new();
        public IEnumerable<T> AddExtraWorkdayManual<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldManualFollowupDoc, new();        
        public (bool isCompleted, string message) SaveDocumentToDatabase(IConnectionManagement connectionManagement, MasterFollowupDocument followupDocument);
    }
}
