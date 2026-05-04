// ==========================================================
// File: CustomerController.cs
// Role: Controller (MVC)
// Description: Manages customer-related data flows between 
// the Views and the CustomerService.
// ==========================================================
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick.Controllers
{
    public class CustomerController
    {
        private readonly CustomerService _customerService;

        // Dieu phoi nghiep vu Customer Controller giua man hinh va tang service.
        public CustomerController()
        {
            _customerService = new CustomerService();
        }

        // Lay du lieu/ket qua cho Get All Customers tu tang xu ly phu hop.
        public Task<List<CustomerModel>> GetAllCustomersAsync()
        {
            return _customerService.GetAllCustomersAsync();
        }

        // Lay du lieu/ket qua cho Get Tier Counts tu tang xu ly phu hop.
        public Task<CustomerTierCountsModel> GetTierCountsAsync()
        {
            return _customerService.GetTierCountsAsync();
        }

        // Lay du lieu/ket qua cho Get Revenue Summary tu tang xu ly phu hop.
        public Task<CustomerRevenueSummaryModel> GetRevenueSummaryAsync()
        {
            return _customerService.GetRevenueSummaryAsync();
        }

        // Lay du lieu/ket qua cho Get Today Occupancy Pct tu tang xu ly phu hop.
        public Task<int> GetTodayOccupancyPctAsync()
        {
            return _customerService.GetTodayOccupancyPctAsync();
        }

        // Lay du lieu/ket qua cho Get Membership Summary tu tang xu ly phu hop.
        public Task<MembershipSummaryModel> GetMembershipSummaryAsync()
        {
            return _customerService.GetMembershipSummaryAsync();
        }
    }
}


