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
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ProdInfoSys.Classes
{
    public class MongoDbModel
    {
        private NewDocument _newFollowupDocument;
        public MongoDbModel(NewDocument NewFollowupDocument)
        {
            _newFollowupDocument = NewFollowupDocument;
        }
        /// <summary>
        /// Teljes followup dokumentum létrehozása
        /// </summary>
        public void MakeDataModell(List<ErpMachineCenter> erpMachineCenters)
        {
            //Saving data
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);

            if (databaseCollection.Find(d => d.DocumentName == _newFollowupDocument.DocumentName).Any())
            {
                MessageBox.Show($"A megadott dokumentumnév ({_newFollowupDocument.DocumentName}) már létezik","MongoDbModel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            

            List<HeadCountFollowupDocument> hcList = new List<HeadCountFollowupDocument>();
            foreach (var workday in _newFollowupDocument.WorkdayList)
            {
                var hc = new HeadCountFollowupDocument()
                {
                    Workday = workday,
                    //DailyHCPlanAvg = _newFollowupDocument.HeadcountPlanNet,
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
                        var newDay = new MachineFollowupDocument() {
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
    }
}
