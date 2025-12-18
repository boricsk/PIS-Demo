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
    /// <summary>
    /// Provides management functionality for follow-up documents, including data aggregation and preparation for report
    /// views based on the caller context.
    /// </summary>
    /// <remarks>This class is typically used to retrieve and organize follow-up document data for reporting
    /// scenarios, adapting the output according to the specified caller entity (such as QRQC or production). It
    /// encapsulates logic for selecting relevant reports, extracting key metrics, and preparing collections for
    /// consumption by reporting views. Thread safety is not guaranteed; create a new instance per usage
    /// context.</remarks>
    public class FollowupDocManagement
    {
        private MasterFollowupDocument _doc;
        private string _selectedReportName;
        private EnumCallerEntity _callerEntity;
        private List<string> _affectedManualWorkcenters = new List<string>(); 

        private decimal ManualProcessAvgEffForQrqc = 0;

        /// <summary>
        /// Initializes a new instance of the FollowupDocManagement class with the specified document, report name, and
        /// caller entity.
        /// </summary>
        /// <param name="doc">The master follow-up document to be managed. Cannot be null.</param>
        /// <param name="selectedReportName">The name of the report to be selected for follow-up operations. Cannot be null or empty.</param>
        /// <param name="callerEntity">The entity that is initiating the follow-up management operation.</param>
        public FollowupDocManagement(MasterFollowupDocument doc, string selectedReportName, EnumCallerEntity callerEntity)
        {
            _doc = doc;
            _selectedReportName = selectedReportName;
            _callerEntity = callerEntity;
            _affectedManualWorkcenters = SetupManagement.LoadSetupData().AvgManualWorkcenters.ToList();
        }

        /// <summary>
        /// Retrieves and assembles metadata required for displaying nested report views based on the current caller
        /// entity and selected report.
        /// </summary>
        /// <remarks>The returned metadata includes different sets of data depending on whether the caller
        /// entity is QRQC or PROD. This method aggregates information such as status reports, machine and manual
        /// process metrics, planning and cost data, and daily plan quantities to support detailed reporting
        /// views.</remarks>
        /// <returns>A <see cref="FollowupMetadata"/> object containing the relevant status reports, planning data, efficiency
        /// metrics, and other details for the selected report and caller context.</returns>
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
