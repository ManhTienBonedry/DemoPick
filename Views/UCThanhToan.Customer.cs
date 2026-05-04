using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private async void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            string search = txtCustomerPhone.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                _currentDiscountPct = 0;
                _currentCustomerId = 0;
                _isFixedCustomer = false;
                lblCustomerInfo.Text = "Khách lẻ (Không áp dụng thẻ)";
                lblCustomerInfo.ForeColor = Color.Gray;
                UpdateTotals();
                return;
            }

            try
            {
                var customer = await _controller.SearchCustomerAsync(search);
                if (customer != null && customer.MemberId > 0)
                {
                    _currentCustomerId = customer.MemberId;
                    string name = customer.FullName ?? "";
                    string tier = MembershipTierHelper.NormalizeTier(customer.Tier);
                    _isFixedCustomer = customer.IsFixed;

                    _currentDiscountPct = 0m;

                    string status = _isFixedCustomer ? "Cố định" : "Thành viên";
                    lblCustomerInfo.Text = string.Format("✓ {0} ({1})", name, status);
                    lblCustomerInfo.ForeColor = Color.DarkGreen;

                    if (_isFixedCustomer)
                    {
                        lblCustomerInfo.Text += " | CO DINH";
                    }
                }
                else
                {
                    _currentDiscountPct = 0;
                    _currentCustomerId = 0;
                    _isFixedCustomer = false;
                    lblCustomerInfo.Text = "Khong tim thay khach hang nay.";
                    lblCustomerInfo.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("ThanhToan Customer Error", ex, "UCThanhToan.BtnSearchCustomer_Click");
            }

            UpdateTotals();
            ReloadPaymentHistory();
        }
    }
}


