using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCThanhToan : UserControl
    {
        private readonly CustomerService _customerService;
        private decimal _cartTotal = 0;
        private string _selectedCourtName = "";
        private decimal _currentDiscountPct = 0;
        private int _currentCustomerId = 0;
        private bool _isFixedCustomer = false;
        private DemoPick.Models.BookingModel _currentBooking;
        private DemoPick.Models.CourtModel _selectedCourt;
        private decimal _lastDiscountAmount = 0m;
        private decimal _lastFinalTotal = 0m;

        public UCThanhToan()
        {
            InitializeComponent();
            if (DesignModeUtil.IsDesignMode(this)) return;

            _customerService = new CustomerService();

            SetupListView();
            btnSearchCustomer.Click += BtnSearchCustomer_Click;
            btnCheckout.Click += BtnCheckout_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        public void RefreshOnActivated()
        {
            LoadCourts();
            ResetCheckoutPane();
        }
    }
}
