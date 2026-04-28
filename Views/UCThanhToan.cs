using System;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCThanhToan : UserControl
    {

        private readonly DemoPick.Controllers.ThanhToanController _controller;
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


            _controller = new DemoPick.Controllers.ThanhToanController();

            SetupListView();
            btnSearchCustomer.Click += BtnSearchCustomer_Click;
            btnCheckout.Click += BtnCheckout_Click;
            btnCancel.Click += BtnCancel_Click;

            InitializeReprintButton();
            InitializePaymentHistoryPanel();
            UpdateReprintButtonState();
            ReloadPaymentHistory();
        }

        public void RefreshOnActivated()
        {
            LoadCourts();
            ResetCheckoutPane();
            UpdateReprintButtonState();
            ReloadPaymentHistory();
        }

        private void ucPaymentHistoryPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
