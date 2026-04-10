using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCThanhToan
    {
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
                var customer = await _customerService.FindCheckoutCustomerAsync(search);
                if (customer != null && customer.MemberId > 0)
                {
                    _currentCustomerId = customer.MemberId;
                    string name = customer.FullName ?? "";
                    string tier = (customer.Tier ?? "").ToLowerInvariant();
                    _isFixedCustomer = customer.IsFixed;

                    if (tier.Contains("vip") || tier.Contains("vàng") || tier == "gold")
                    {
                        _currentDiscountPct = 0.10m;
                        lblCustomerInfo.Text = $"✓ {name} (VIP). Giảm 10%.";
                        lblCustomerInfo.ForeColor = Color.FromArgb(255, 160, 0);
                    }
                    else if (tier.Contains("bạc") || tier == "silver")
                    {
                        _currentDiscountPct = 0.05m;
                        lblCustomerInfo.Text = $"✓ {name} (Bạc). Giảm 5%.";
                        lblCustomerInfo.ForeColor = Color.FromArgb(76, 175, 80);
                    }
                    else
                    {
                        _currentDiscountPct = 0.02m;
                        lblCustomerInfo.Text = $"✓ {name} (Đồng). Giảm 2%.";
                        lblCustomerInfo.ForeColor = Color.FromArgb(31, 41, 55);
                    }
                    if (_isFixedCustomer)
                    {
                        lblCustomerInfo.Text += " | CỐ ĐỊNH";
                    }
                }
                else
                {
                    _currentDiscountPct = 0;
                    _currentCustomerId = 0;
                    _isFixedCustomer = false;
                    lblCustomerInfo.Text = "⚠ Không tìm thấy khách hàng này!";
                    lblCustomerInfo.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("ThanhToan Customer Error", ex, "UCThanhToan.BtnSearchCustomer_Click");
            }

            UpdateTotals();
        }
    }
}
