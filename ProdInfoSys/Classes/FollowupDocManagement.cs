using DocumentFormat.OpenXml.Drawing;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.Models.StatusReportModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProdInfoSys.Classes
{
    public class FollowupDocManagement
    {
        private MasterFollowupDocument _doc;
        private string _selectedReportName;
        private EnumCallerEntity _callerEntity;
        private List<string> _affectedManualWorkcenters = new List<string>(); 

        private decimal ManualProcessAvgEffForQrqc = 0;
        public FollowupDocManagement(MasterFollowupDocument doc, string selectedReportName, EnumCallerEntity callerEntity)
        {
            _doc = doc;
            _selectedReportName = selectedReportName;
            _callerEntity = callerEntity;
            _affectedManualWorkcenters = SetupManagement.LoadSetupData().AvgManualWorkcenters.ToList();
        }

        public FollowupMetadata GetDataForNestedReportViews()
        {
            FollowupMetadata ret = new FollowupMetadata();
            var _prodMeetingWorkcenterList = SetupManagement.GetProdMeetingWorkcenterList();            
            
            if (_callerEntity == EnumCallerEntity.QRQC)
            {
                ret.AllStatusReportForQrqc = new ObservableCollection<StatusReportQrqc>(_doc.StatusReportsQRQC.ToList());
                ret.SelectedStatusReportQrqc = _doc.StatusReportsQRQC.Where(x => x.ReportName == _selectedReportName).FirstOrDefault();
                ret.ManualRejectRatio = ret.SelectedStatusReportQrqc.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();
                ret.MachineRejectRatio = ret.SelectedStatusReportQrqc.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCMachineProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();
                ret.InspectionRejectRatio = ret.SelectedStatusReportQrqc.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCInscpectionProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();
                ret.MachineStatusReports = new ObservableCollection<StatusReportMachineList>(_doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.MachineData));
                
                ret.KftEfficiency = new ObservableCollection<decimal>(ret.MachineStatusReports.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgEfficiency).ToList());
                ManualProcessAvgEffForQrqc = ret.MachineStatusReports.Where(s => _affectedManualWorkcenters.Contains(s.Workcenter)).Select(s => s.AvgEfficiency).Average();
                ret.SubconEfficiency = new ObservableCollection<decimal>(ret.MachineStatusReports.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgEfficiencySubcon).ToList());
                ret.KftEfficiency.Add(ManualProcessAvgEffForQrqc);
                ret.AffectedMachineStatusReportsForQrqcMeeting = new ObservableCollection<StatusReportMachineList>(ret.MachineStatusReports.ToList());

                ret.PlanningDataOfReport = _doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).Select(s => s.PlansData).FirstOrDefault();
                ret.WorkcenterFollowupData = new ObservableCollection<StatusReportMachineList>(_doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.MachineData));
                ret.PlanChangeDetails = new ObservableCollection<PlanningMasterData>(ret.PlanningDataOfReport.PlanChanges);
                ret.WorkcenterFollowupData = new ObservableCollection<StatusReportMachineList>(_doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.MachineData));
                ret.PlannedSamplePrice = ret.PlanningDataOfReport.Sample.ToString("N2");
                ret.ProdPlanPrice = ret.PlanningDataOfReport.OutputPlanSales.ToString("N2");
                ret.ProdPlanDcPrice = ret.PlanningDataOfReport.OutputPlanDc.ToString("N2");
                ret.ProdPlanMaterialPrice = ret.PlanningDataOfReport.MaterialCost.ToString("N2");
                ret.ShipoutPlanPrice = ret.PlanningDataOfReport.SalesPlan.ToString("N2");
                ret.ProdPlanTotalPrice = ret.PlanningDataOfReport.TtlPlan.ToString("N2");
                ret.RepackDailyPlannedQty = new ObservableCollection<DailyPlan>(_doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.RepackDailyPlans));
                ret.KftDailyPlannedQty = new ObservableCollection<DailyPlan>(_doc.StatusReportsQRQC.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.KftDailyPlans));
                var _stopTimes = _doc?.StatusReportsQRQC?.Where(r => r.ReportName == _selectedReportName)
                    .SelectMany(s => s.StopTimes ?? Enumerable.Empty<StopTime>()) ?? Enumerable.Empty<StopTime>();
                if (_stopTimes.Any())
                {
                    ret.StopTimes = new ObservableCollection<StopTime>(_doc.StatusReportsQRQC.Where(r => r.ReportName == _selectedReportName).SelectMany(s => s.StopTimes));
                }
            }

            if (_callerEntity == EnumCallerEntity.PROD)
            {
                ret.AllStatusReportForProd = new ObservableCollection<StatusReport>(_doc.StatusReports.ToList());
                ret.PlanOutputValues = ret.AllStatusReportForProd.Select(s => s.PlansData).Select(s => s.OutputPlanSales).ToList();
                ret.RmCostValues = ret.AllStatusReportForProd.Select(p => p.PlansData).Select(p => p.MaterialCost).ToList();
                ret.RepackRmCostValues = ret.AllStatusReportForProd.Select(p => p.PlansData).Select(p => p.RepackMaterialCost).ToList();
                ret.KftRmCostValues = ret.RmCostValues.Zip(ret.RepackRmCostValues, (a,b) => a - b).ToList();

                ret.SalesOutputValues = ret.AllStatusReportForProd.Select(s => s.PlansData).Select(s => s.SalesPlan).ToList();                
                ret.SelectedStatusReportProd = _doc.StatusReports.Where(x => x.ReportName == _selectedReportName).FirstOrDefault();
                ret.ManualRejectRatio = ret.SelectedStatusReportProd.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();
                ret.MachineRejectRatio = ret.SelectedStatusReportProd.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCMachineProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();
                ret.InspectionRejectRatio = ret.SelectedStatusReportProd.MachineData.Where(s => s.WorkcenterType == EnumMachineType.FFCInscpectionProcess.ToString()).Select(s => s.AvgRejectRatio).ToList();

                ret.MachineStatusReports = new ObservableCollection<StatusReportMachineList>(_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.MachineData));
                ret.AffectedMachineStatusReportsForProdMeeting = new ObservableCollection<StatusReportMachineList>(ret.MachineStatusReports.Where(s => _prodMeetingWorkcenterList.Contains(s.Workcenter)).ToList());
                ret.KftEfficiency = new ObservableCollection<decimal>(ret.MachineStatusReports.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgEfficiency).ToList());
                ret.SubconEfficiency = new ObservableCollection<decimal>(ret.MachineStatusReports.Where(s => s.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(s => s.AvgEfficiencySubcon).ToList());
                
                ret.PlanningDataOfReport =_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).Select(s => s.PlansData).FirstOrDefault();
                ret.PlanChangeDetails = new ObservableCollection<PlanningMasterData>(ret.PlanningDataOfReport.PlanChanges);
                ret.ShipoutPlanDetails = new ObservableCollection<ShipoutPlan>(_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.ShipoutPlan));
                ret.WorkcenterFollowupData = new ObservableCollection<StatusReportMachineList>(_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.MachineData));
                ret.PlannedSamplePrice = ret.PlanningDataOfReport.Sample.ToString("N2");
                ret.ProdPlanPrice = ret.PlanningDataOfReport.OutputPlanSales.ToString("N2");
                ret.ProdPlanDcPrice = ret.PlanningDataOfReport.OutputPlanDc.ToString("N2");
                ret.ProdPlanMaterialPrice = ret.PlanningDataOfReport.MaterialCost.ToString("N2");
                ret.ProdPlanKftMaterialPrice = (ret.PlanningDataOfReport.MaterialCost - ret.PlanningDataOfReport.RepackMaterialCost).ToString("N2");
                ret.ProdPlanRepackMaterialPrice = ret.PlanningDataOfReport.RepackMaterialCost.ToString("N2");
                
                
                ret.ShipoutPlanPrice = ret.PlanningDataOfReport.SalesPlan.ToString("N2");
                ret.ShipoutPlanDcPrice = ret.PlanningDataOfReport.SalesPlanDC.ToString("N2");
                ret.DcMovement = ret.PlanningDataOfReport.DCMovement.ToString("N2");                
                ret.ProdPlanTotalPrice = ret.PlanningDataOfReport.TtlPlan.ToString("N2");
                ret.RepackDailyPlannedQty = new ObservableCollection<DailyPlan>(_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.RepackDailyPlans));
                ret.KftDailyPlannedQty = new ObservableCollection<DailyPlan>(_doc.StatusReports.Where(s => s.ReportName == _selectedReportName).SelectMany(s => s.KftDailyPlans));
            }           

            return ret;
        }
    }
}
