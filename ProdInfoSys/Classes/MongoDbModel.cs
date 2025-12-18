using MongoDB.Driver;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides functionality to create and store a complete follow-up document in a MongoDB database based on the
    /// specified input data.
    /// </summary>
    /// <remarks>This class is intended to facilitate the construction and persistence of follow-up documents,
    /// aggregating data from various sources such as machine centers and workdays. It is typically used in scenarios
    /// where a new follow-up document needs to be generated and saved, ensuring that document names are unique within
    /// the database. Thread safety is not guaranteed; create a new instance for each operation if used in
    /// multi-threaded environments.</remarks>
    public class MongoDbModel
    {
        private NewDocument _newFollowupDocument;
        public MongoDbModel(NewDocument NewFollowupDocument)
        {
            _newFollowupDocument = NewFollowupDocument;
        }
        
        /// <summary>
        /// Creates and saves a new master follow-up document using the specified list of ERP machine centers and the
        /// current follow-up data. If a document with the same name already exists, the operation is canceled and an
        /// error message is displayed.
        /// </summary>
        /// <remarks>If a document with the same name as the current follow-up document already exists in
        /// the database, no new document is created and an error message is shown to the user. This method displays
        /// informational or error messages to the user via message boxes during execution.</remarks>
        /// <param name="erpMachineCenters">A list of ERP machine center objects that provide parallel capacity and workcenter information used to
        /// populate the new follow-up document. Cannot be null.</param>
        public void MakeDataModell(List<ErpMachineCenter> erpMachineCenters)
        {
            try
            {
                //Saving data
                ConnectionManagement conMgmnt = new ConnectionManagement();
                var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);

                if (databaseCollection.Find(d => d.DocumentName == _newFollowupDocument.DocumentName).Any())
                {
                    MessageBox.Show($"A megadott dokumentumnév ({_newFollowupDocument.DocumentName}) már létezik", "MongoDbModel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<HeadCountFollowupDocument> hcList = new List<HeadCountFollowupDocument>();
                foreach (var workday in _newFollowupDocument.WorkdayList)
                {
                    var hc = new HeadCountFollowupDocument()
                    {
                        Workday = workday,
                        ShiftLen = _newFollowupDocument.ManualShiftLength,
                        ShiftNum = _newFollowupDocument.ManualShiftNumber,
                        NettoHCPlan = _newFollowupDocument.HeadcountPlanNet,
                        HCPlan = _newFollowupDocument.HeadcountPlanBr

                    };
                    hcList.Add(hc);
                }

                List<MachineFollowupDocument> machineFollowup = new List<MachineFollowupDocument>();
                List<Machines> machines = new List<Machines>();

                List<InspectionFollowupDocument> inspectionFollowup = new List<InspectionFollowupDocument>();
                List<Inspection> inspections = new List<Inspection>();

                List<ManualFollowupDocument> manualFollowup = new List<ManualFollowupDocument>();
                List<Manual> manuals = new List<Manual>();

                //Munkanapok listájának létrehozása. 
                foreach (var workday in _newFollowupDocument.WorkdayList)
                {
                    var mWorkdayList = new MachineFollowupDocument()
                    {
                        Workday = workday,
                    };
                    machineFollowup.Add(mWorkdayList);
                }

                foreach (var workday in _newFollowupDocument.WorkdayList)
                {
                    var mWorkdayList = new InspectionFollowupDocument()
                    {
                        Workday = workday,
                    };
                    inspectionFollowup.Add(mWorkdayList);
                }

                foreach (var workday in _newFollowupDocument.WorkdayList)
                {
                    var mWorkdayList = new ManualFollowupDocument()
                    {
                        Workday = workday,
                    };
                    manualFollowup.Add(mWorkdayList);
                }

                //Minden gépcsoport ami az enum típusba esik be lesz téve a Machines listájába és hozzá lesz adva a 
                //munkanaplista a követési adatokkal.
                foreach (var workcenter in _newFollowupDocument.MachineCenters)
                {
                    if (workcenter.MachineType == EnumMachineType.FFCMachineProcess)
                    {
                        int currentComulatedQty = 0;
                        //FirstOfDefault, mert a select listát ad vissza
                        int dailyPlan = _newFollowupDocument.MachineCenters.Where(m => m.Workcenter == workcenter.Workcenter).Select(m => m.DailyPlan).FirstOrDefault();
                        decimal paralellCapacity = erpMachineCenters.Where(m => m.Workcenter == workcenter.Workcenter).Select(w => w.ParalellCaps).FirstOrDefault();
                        var induvidualFollowups = new List<MachineFollowupDocument>();
                        foreach (var day in machineFollowup)
                        {
                            var newDay = new MachineFollowupDocument()
                            {
                                Workday = day.Workday,
                                DailyPlan = dailyPlan,
                                ComulatedPlan = currentComulatedQty + dailyPlan,
                                NumberOfWorkdays = _newFollowupDocument.Workdays,
                                AvailOperatingHour = _newFollowupDocument.MachineShiftNumber * (double)_newFollowupDocument.MachineShiftLength * (double)paralellCapacity,
                            };
                            currentComulatedQty = newDay.ComulatedPlan;
                            induvidualFollowups.Add(newDay);
                        }

                        Machines m = new Machines()
                        {
                            Workcenter = workcenter.Workcenter,
                            MachineFollowupDocuments = induvidualFollowups
                        };
                        machines.Add(m);
                    }

                    if (workcenter.MachineType == EnumMachineType.FFCInspectionMachine || workcenter.MachineType == EnumMachineType.FFCManualInspection)
                    {
                        int currentComulatedQty = 0;
                        int dailyPlan = _newFollowupDocument.MachineCenters.Where(m => m.Workcenter == workcenter.Workcenter).Select(m => m.DailyPlan).FirstOrDefault();
                        var induvidualFollowups = new List<InspectionFollowupDocument>();
                        decimal paralellCapacity = erpMachineCenters.Where(m => m.Workcenter == workcenter.Workcenter).Select(w => w.ParalellCaps).FirstOrDefault();

                        foreach (var day in inspectionFollowup)
                        {
                            var newDay = new InspectionFollowupDocument()
                            {
                                DailyPlan = dailyPlan,
                                Workday = day.Workday,
                                ComulatedPlan = currentComulatedQty + dailyPlan,
                                NumberOfWorkdays = _newFollowupDocument.Workdays,
                                AvailOperatingHour = _newFollowupDocument.MachineShiftNumber * (double)_newFollowupDocument.MachineShiftLength * (double)paralellCapacity,
                            };
                            currentComulatedQty = newDay.ComulatedPlan;
                            induvidualFollowups.Add(newDay);
                        }

                        Inspection i = new Inspection()
                        {
                            Workcenter = workcenter.Workcenter,
                            InspectionFollowupDocuments = induvidualFollowups
                        };
                        inspections.Add(i);
                    }

                    if (workcenter.MachineType == EnumMachineType.FFCManualProcess)
                    {
                        int currentComulatedQty = 0;
                        var induvidualFollowups = new List<ManualFollowupDocument>();
                        int dailyPlan = _newFollowupDocument.MachineCenters.Where(m => m.Workcenter == workcenter.Workcenter).Select(m => m.DailyPlan).FirstOrDefault();
                        foreach (var day in manualFollowup)
                        {
                            var newDay = new ManualFollowupDocument()
                            {
                                Workday = day.Workday,
                                DailyPlan = dailyPlan,
                                ComulatedPlan = currentComulatedQty + dailyPlan,
                                NumberOfWorkdays = _newFollowupDocument.Workdays,
                            };
                            currentComulatedQty = newDay.ComulatedPlan;
                            induvidualFollowups.Add(newDay);
                        }

                        Manual man = new Manual()
                        {
                            Workcenter = workcenter.Workcenter,
                            MaualFollowupDocuments = induvidualFollowups,
                        };
                        manuals.Add(man);
                    }
                }

                MasterFollowupDocument newDocument = new MasterFollowupDocument()
                {

                    DocumentName = _newFollowupDocument.DocumentName,
                    Workdays = _newFollowupDocument.Workdays,
                    ShiftLenManual = _newFollowupDocument.ManualShiftLength,
                    ShiftLenMachine = _newFollowupDocument.MachineShiftLength,
                    ShiftNumberManual = _newFollowupDocument.ManualShiftNumber,
                    ShiftNumMachine = _newFollowupDocument.MachineShiftNumber,
                    AbsenseRatio = _newFollowupDocument.AbsenseRatio / 100,
                    PlanName = _newFollowupDocument.PlanName,
                    Headcount = hcList,
                    MachineFollowups = machines,
                    InspectionFollowups = inspections,
                    ManualFollowups = manuals,
                    StatusReports = null,
                    StartDate = _newFollowupDocument.StartDate.ToString("yyyy-MM-dd"),
                    FinishDate = _newFollowupDocument.FinishDate.ToString("yyyy-MM-dd")

                };
                MongoDbOperations<MasterFollowupDocument> dbo = new MongoDbOperations<MasterFollowupDocument>(databaseCollection);
                dbo.AddNewDocument(newDocument);

                MessageBox.Show($"Dokumentum ({_newFollowupDocument.DocumentName}) létrehozva!", "MongoDbModel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Mongo modell létrehozás", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
