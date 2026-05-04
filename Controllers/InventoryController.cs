// ==========================================================
// File: InventoryController.cs
// Role: Controller (MVC)
// Description: Manages inventory data operations, providing 
// KPI metrics and item lists to the UI.
// ==========================================================
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick.Controllers
{
    public class InventoryController
    {
        private readonly InventoryService _inventoryService;

        // Dieu phoi nghiep vu Inventory Controller giua man hinh va tang service.
        public InventoryController()
        {
            _inventoryService = new InventoryService();
        }

        // Lay du lieu/ket qua cho Get Inventory Kpis tu tang xu ly phu hop.
        public Task<InventoryKpiModel> GetInventoryKpisAsync()
        {
            return _inventoryService.GetInventoryKpisAsync();
        }



        // Lay du lieu/ket qua cho Get Inventory Items tu tang xu ly phu hop.
        public Task<List<InventoryItemModel>> GetInventoryItemsAsync()
        {
            return _inventoryService.GetInventoryItemsAsync();
        }

        // Lay du lieu/ket qua cho Get Recent Transactions tu tang xu ly phu hop.
        public Task<List<TransactionModel>> GetRecentTransactionsAsync()
        {
            return _inventoryService.GetRecentTransactionsAsync();
        }

        // Tao hoac tinh ra du lieu Build Smart Insights tu cac thong tin dau vao hien co.
        public InventorySmartInsightsModel BuildSmartInsights(IReadOnlyCollection<InventoryItemModel> items)
        {
            return _inventoryService.BuildSmartInsights(items);
        }
    }
}


