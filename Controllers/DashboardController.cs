// ==========================================================
// File: DashboardController.cs
// Role: Controller (MVC)
// Description: Coordinates data for the main dashboard view.
// ==========================================================
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick.Controllers
{
    public class DashboardController
    {
        private readonly DashboardService _dashboardService;

        // Dieu phoi nghiep vu Dashboard Controller giua man hinh va tang service.
        public DashboardController()
        {
            _dashboardService = new DashboardService();
        }

        // Lay du lieu/ket qua cho Get Metrics tu tang xu ly phu hop.
        public Task<DashboardMetricsModel> GetMetricsAsync()
        {
            return _dashboardService.GetMetricsAsync();
        }

        // Lay du lieu/ket qua cho Get Revenue Trend Last7 Days tu tang xu ly phu hop.
        public Task<List<TrendPointModel>> GetRevenueTrendLast7DaysAsync()
        {
            return _dashboardService.GetRevenueTrendLast7DaysAsync();
        }

        // Lay du lieu/ket qua cho Get Top Courts Revenue tu tang xu ly phu hop.
        public Task<List<NamedRevenueModel>> GetTopCourtsRevenueAsync()
        {
            return _dashboardService.GetTopCourtsRevenueAsync();
        }

        // Lay du lieu/ket qua cho Get Recent Activity tu tang xu ly phu hop.
        public Task<List<DashboardActivityModel>> GetRecentActivityAsync(int limit)
        {
            return _dashboardService.GetRecentActivityAsync(limit);
        }


    }
}


