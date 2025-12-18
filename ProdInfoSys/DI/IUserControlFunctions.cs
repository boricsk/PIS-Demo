using ProdInfoSys.Interfaces;
using ProdInfoSys.Models.NonRelationalModels;

namespace ProdInfoSys.DI
{
    public interface IUserControlFunctions
    {
        /// <summary>
        /// Calculates a cumulative output value based on the provided follow-up documents.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection. Must implement the IHasFieldFollowupDoc interface.</typeparam>
        /// <param name="followupDocument">A collection of follow-up document items to be processed. Cannot be null.</param>
        /// <returns>An integer representing the cumulative output calculated from the follow-up documents.</returns>
        public int GetComulatedOut<T>(IEnumerable<T> followupDocument) where T : IHasFieldFollowupDoc;

        public IEnumerable<T> AddExtraWorkday<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldHCFollowupDoc, new();
        public IEnumerable<T> AddExtraWorkdayMachine<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldMachineFollowupDoc, new();
        public IEnumerable<T> AddExtraWorkdayManual<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldManualFollowupDoc, new();
        public (bool isCompleted, string message) SaveDocumentToDatabase(IConnectionManagement connectionManagement, MasterFollowupDocument followupDocument);
    }
}
