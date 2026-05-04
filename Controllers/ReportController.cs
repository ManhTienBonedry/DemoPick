// ==========================================================
// File: ReportController.cs
// Role: Controller (MVC)
// Description: Handles report generation, metrics, and KPI 
// formatting for the Report views.
// ==========================================================
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick.Controllers
{
    public class ReportController
    {
        private readonly ReportService _reportService;

        // Dieu phoi nghiep vu Report Controller giua man hinh va tang service.
        public ReportController()
        {
            _reportService = new ReportService();
        }

        // Lay du lieu/ket qua cho Get Kpis tu tang xu ly phu hop.
        public Task<ReportKpiModel> GetKpisAsync(System.DateTime from, System.DateTime to, int days)
        {
            return _reportService.GetKpisAsync(from, to, days);
        }

        // Lay du lieu/ket qua cho Get Booking Hour Heatmap tu tang xu ly phu hop.
        public Task<List<ReportHeatmapPointModel>> GetBookingHourHeatmapAsync(System.DateTime from, System.DateTime to)
        {
            return _reportService.GetBookingHourHeatmapAsync(from, to);
        }

        // Lay du lieu/ket qua cho Get Booking Ops tu tang xu ly phu hop.
        public Task<ReportBookingOpsModel> GetBookingOpsAsync(System.DateTime from, System.DateTime to)
        {
            return _reportService.GetBookingOpsAsync(from, to);
        }

        // Lay du lieu/ket qua cho Get Top Courts tu tang xu ly phu hop.
        public Task<List<TopCourtModel>> GetTopCourtsAsync(System.DateTime from, System.DateTime to)
        {
            return _reportService.GetTopCourtsAsync(from, to);
        }

        // Lay du lieu/ket qua cho Get Top Courts Revenue tu tang xu ly phu hop.
        public Task<List<NamedRevenueModel>> GetTopCourtsRevenueAsync(System.DateTime from, System.DateTime to)
        {
            return _reportService.GetTopCourtsRevenueAsync(from, to);
        }
    }
}


