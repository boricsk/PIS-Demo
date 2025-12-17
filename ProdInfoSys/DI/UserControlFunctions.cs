using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.Interfaces;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.DI
{
    public class UserControlFunctions : IUserControlFunctions
    {
        private IUserDialogService _dialogs;
        public UserControlFunctions() { }
        public UserControlFunctions(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
        }

        public IEnumerable<T> AddExtraWorkdayManual<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldManualFollowupDoc, new()
        {
            if (extraWorkday.DayOfWeek != DayOfWeek.Sunday && extraWorkday.DayOfWeek != DayOfWeek.Saturday)
            {
                //_dialogs.ShowErrorInfo($"Csak hétvége választható ki!", "HeadCountViewModel");
                //return followupDocument;
            }

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

        public IEnumerable<T> AddExtraWorkdayMachine<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldMachineFollowupDoc, new()
        {
            if (extraWorkday.DayOfWeek != DayOfWeek.Sunday && extraWorkday.DayOfWeek != DayOfWeek.Saturday)
            {
                //_dialogs.ShowErrorInfo($"Csak hétvége választható ki!", "HeadCountViewModel");
                //return followupDocument;
            }

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

        public IEnumerable<T> AddExtraWorkday<T>(IEnumerable<T> followupDocument, DateTime extraWorkday) where T : IHasFieldHCFollowupDoc, new()
        {
            //if (extraWorkday.DayOfWeek != DayOfWeek.Sunday && extraWorkday.DayOfWeek != DayOfWeek.Saturday)
            //{
            //    _dialogs.ShowErrorInfo($"Csak hétvége választható ki!", "HeadCountViewModel");
            //    return followupDocument;
            //}

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

        public int GetComulatedOut<T>(IEnumerable<T> followupDocument) where T : IHasFieldFollowupDoc => followupDocument.Select(x => x.OutputSum).Sum();

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
