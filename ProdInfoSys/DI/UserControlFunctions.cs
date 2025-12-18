using MongoDB.Driver;
using ProdInfoSys.Interfaces;
using ProdInfoSys.Models.NonRelationalModels;

namespace ProdInfoSys.DI
{
    /// <summary>
    /// Provides utility functions for managing user control follow-up documents, including operations for adding extra
    /// workdays and saving documents to the database.
    /// </summary>
    /// <remarks>This class offers generic methods to support different types of follow-up documents by
    /// leveraging interface constraints. It is intended to be used in scenarios where user-driven modifications to
    /// workday-related data are required, such as manual or machine-based follow-up tracking. Thread safety is not
    /// guaranteed; callers should ensure appropriate synchronization if used in multi-threaded environments.</remarks>
    public class UserControlFunctions : IUserControlFunctions
    {
        private IUserDialogService _dialogs;
        public UserControlFunctions() { }
        public UserControlFunctions(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
        }

        /// <summary>
        /// Adds a new manual follow-up workday entry to the collection if the specified date is a weekend and not
        /// already present.
        /// </summary>
        /// <remarks>If the specified date is not a weekend or is already present in the collection, no
        /// new entry is added. The method does not modify the original collection but returns a new sequence with the
        /// additional item if applicable.</remarks>
        /// <typeparam name="T">The type of the follow-up document. Must implement the IHasFieldManualFollowupDoc interface and have a
        /// parameterless constructor.</typeparam>
        /// <param name="followupDocument">The collection of follow-up document items to which the extra workday will be added. Cannot be null.</param>
        /// <param name="extraWorkday">The date to add as an extra manual workday. Must be a Saturday or Sunday.</param>
        /// <returns>A new collection containing the original items and the added extra workday entry if the date is valid and
        /// not already present; otherwise, the original collection.</returns>
        public IEnumerable<T> AddExtraWorkdayManual<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldManualFollowupDoc, new()
        {
            if (followupDocument is null)
                return followupDocument;

            var currentWorkdays = followupDocument.Select(s => s.Workday).ToList();

            if (currentWorkdays.Contains(DateOnly.FromDateTime(extraWorkday)))
            {
                _dialogs.ShowErrorInfo($"Ez a nap már a listában van!", "HeadCountViewModel");
                return followupDocument;
            }

            var first = followupDocument.FirstOrDefault();
            if (first is null)
                return followupDocument;

            var newItem = new T
            {
                Workday = DateOnly.FromDateTime(extraWorkday),
            };

            return followupDocument.Append(newItem);
        }

        /// <summary>
        /// Adds a new machine follow-up document entry for the specified extra workday to the provided collection, if
        /// the workday is not already present.
        /// </summary>
        /// <remarks>If the specified extra workday is not a Saturday or Sunday, no entry is added. If the
        /// extra workday already exists in the collection, no duplicate is added. The new entry copies the available
        /// operating hour from the first item in the collection.</remarks>
        /// <typeparam name="T">The type of the follow-up document, which must implement the IHasFieldMachineFollowupDoc interface and have
        /// a parameterless constructor.</typeparam>
        /// <param name="followupDocument">The collection of existing machine follow-up document entries to which the extra workday entry may be added.
        /// Cannot be null.</param>
        /// <param name="extraWorkday">The date representing the extra workday to add. Only weekend days (Saturday or Sunday) are considered valid.</param>
        /// <returns>A new collection containing the original entries and, if applicable, an additional entry for the specified
        /// extra workday. Returns the original collection if the extra workday is already present or if the input is
        /// null.</returns>
        public IEnumerable<T> AddExtraWorkdayMachine<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldMachineFollowupDoc, new()
        {
            if (followupDocument is null)
                return followupDocument;

            var currentWorkdays = followupDocument.Select(s => s.Workday).ToList();

            if (currentWorkdays.Contains(DateOnly.FromDateTime(extraWorkday)))
            {
                _dialogs.ShowErrorInfo($"Ez a nap már a listában van!", "HeadCountViewModel");
                return followupDocument;
            }

            var first = followupDocument.FirstOrDefault();
            if (first is null)
                return followupDocument;

            var newItem = new T
            {
                Workday = DateOnly.FromDateTime(extraWorkday),
                AvailOperatingHour = first.AvailOperatingHour,
            };

            return followupDocument.Append(newItem);
        }

        /// <summary>
        /// Adds an extra workday entry to the specified collection if it does not already exist.
        /// </summary>
        /// <remarks>If the extra workday already exists in the collection, the method returns the
        /// original collection without modification. The new entry copies the shift length and shift number from the
        /// first item in the collection.</remarks>
        /// <typeparam name="T">The type of the follow-up document. Must implement the IHasFieldHCFollowupDoc interface and have a
        /// parameterless constructor.</typeparam>
        /// <param name="followupDocument">The collection of follow-up document items to which the extra workday will be added. If null, the method
        /// returns null.</param>
        /// <param name="extraWorkday">The date of the extra workday to add. If the date already exists in the collection, no new entry is added.</param>
        /// <returns>A new collection containing the original items and the added extra workday entry, or the original collection
        /// if the extra workday already exists or the input is null.</returns>
        public IEnumerable<T> AddExtraWorkday<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldHCFollowupDoc, new()
        {

            if (followupDocument is null)
                return followupDocument;

            var currentWorkdays = followupDocument.Select(s => s.Workday).ToList();

            if (currentWorkdays.Contains(DateOnly.FromDateTime(extraWorkday)))
            {
                _dialogs.ShowErrorInfo($"Ez a nap már a listában van!", "HeadCountViewModel");
                return followupDocument;
            }

            var first = followupDocument.FirstOrDefault();
            if (first is null)
                return followupDocument;

            var newItem = new T
            {
                Workday = DateOnly.FromDateTime(extraWorkday),
                ShiftLen = first.ShiftLen,
                ShiftNum = first.ShiftNum
            };

            return followupDocument.Append(newItem);
        }

        /// <summary>
        /// Calculates the total sum of the OutputSum values from the specified collection of follow-up documents.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection, which must implement the IHasFieldFollowupDoc interface.</typeparam>
        /// <param name="followupDocument">The collection of follow-up document items whose OutputSum values will be aggregated. Cannot be null.</param>
        /// <returns>The sum of the OutputSum values for all items in the collection. Returns 0 if the collection is empty.</returns>
        public int GetComulatedOut<T>(IEnumerable<T> followupDocument) where T : IHasFieldFollowupDoc => followupDocument.Select(x => x.OutputSum).Sum();

        /// <summary>
        /// Attempts to save the specified follow-up document to the database using the provided connection management
        /// instance.
        /// </summary>
        /// <param name="connectionManagement">An object that manages the database connection and provides access to the target collection.</param>
        /// <param name="followupDocument">The follow-up document to be saved. Cannot be null.</param>
        /// <returns>A tuple containing a boolean value that indicates whether the operation completed successfully, and a
        /// message describing the result or any error encountered.</returns>
        public (bool isCompleted, string message) SaveDocumentToDatabase(IConnectionManagement connectionManagement, MasterFollowupDocument followupDocument)
        {
            (bool isCompleted, string message) ret = (false, string.Empty);
            if (followupDocument != null)
            {
                try
                {
                    connectionManagement.ConnectToDatabase();

                    var collection = connectionManagement.GetCollection<MasterFollowupDocument>(connectionManagement.DbName);
                    var filter = Builders<MasterFollowupDocument>.Filter.Eq(x => x.id, followupDocument.id);
                    collection.ReplaceOne(filter, followupDocument);
                    ret.isCompleted = true;
                    return ret;
                }
                catch (Exception ex)
                {
                    ret.isCompleted = false;
                    ret.message = ex.Message;
                    return ret;
                }
            }
            else
            {
                ret.message = "Mentés sikertelen! _followupDocument vagy HeadCountFollowupDocuments => null";
                ret.isCompleted = false;
                return ret;
            }
        }

    }
}
