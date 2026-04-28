# Click Wiring Checklist (DemoPick)

Mục tiêu:
- Có 1 danh sách để bạn rà soát/gán event thủ công.
- Tránh lỗi **gán 2 lần** (Designer + code) làm handler chạy 2 lần.

## Nguyên tắc

- Chọn **1 nguồn gán event**:
  - **Designer** (trong `*.Designer.cs`) **hoặc**
  - **Code-behind** (constructor sau `InitializeComponent()`).
- Nếu bắt buộc gán bằng code mà sợ trùng, dùng:

```csharp
control.Click -= Handler;
control.Click += Handler;
```

## Audit nhanh: các control có `Cursor = Hand`

### A) Điều hướng chính (Sidebar) — KHÔNG gán thêm trong Designer

File: `../Views/FrmChinh.cs`

- ✅ `btnNavDashboard`, `btnNavDatLich`, `btnNavBanHang`, `btnNavKhachHang`, `btnNavKhoHang`, `btnNavBaoCao`
  - Đã được gán click bằng code qua `BindClick()`.
  - `BindClick()` gán cho **UIPanel** và **mọi control con** trong panel.
  - Checklist: Không cần (và không nên) gán thêm click cho label con trong Designer.

- ✅ `pnlLogo` và các control con
  - Gán click trong `InitModules()`.

- ✅ `pnlAdminAvatar` và các control con
  - Gán click trong `InitModules()`.

### B) Báo cáo

Files:
- `../Views/UCBaoCao.Designer.cs`
- `../Views/UCBaoCao.cs`

- ✅ `btnApplyFilter`
  - Gán `Click` trong code-behind (`UCBaoCao` constructor).
- ✅ `lblTopCourtsViewAll`
  - Gán `Click` trong code-behind.

### C) Đặt lịch

Files:
- `../Views/UCDatLich.Designer.cs`
- `../Views/UCDatLich.cs`

- ✅ `btnPrevDay`, `btnNextDay`
  - Gán `Click` trong code-behind.
- ✅ `pnlCanvas`
  - Gán `MouseClick` / `MouseDoubleClick` trong code-behind để chọn sân, mở đặt sân chơi và đổi ca.
- ✅ `btnDoiCa`
  - Gán `Click` trong code-behind.
- ✅ `dateFilter`
  - Có event `SelectedDateChanged` trong code-behind để đổi ngày và reload timeline.

### D) POS (Bán hàng)

Files:
- `../Views/UCBanHang.Designer.cs`
- `../Views/UCBanHang.cs`

- ✅ `btnAddProduct`, `btnApplyDiscount`, `btnCheckout`
  - Gán `Click` trong code-behind.

### E) Kho hàng

Files:
- `../Views/UCKhoHang.Designer.cs`
- `../Views/UCKhoHang.cs`

- ✅ `btnThemSP`
  - Gán `Click` trong code-behind.

### F) Login

Files:
- `../Views/UCLogin.Designer.cs`
- `../Views/UCLogin.cs`

- ✅ `btnClose` (Label `X`), `btnLogin`, `btnRegister`, `lblRegisterNow`
  - Đang gán event trong Designer.
  - Checklist: **không cần** gán thêm trong constructor.

### G) Register (Đăng ký)

Files:
- `../Views/UCRegister.Designer.cs`
- `../Views/UCRegister.cs`

- ✅ `btnClose` (Label `X`), `btnRegister`, `lblLoginNow`
  - Đang gán trong code-behind (constructor) để đảm bảo bấm được.
  - Checklist: Nếu bạn có gán thêm trong Designer thì coi chừng chạy 2 lần.

### H) User menu

Files:
- `../Views/FrmUserMenu.Designer.cs`
- `../Views/FrmUserMenu.cs`

- ✅ `btnDoiMatKhau`, `btnDangXuat`, `btnThoat`
  - Đang gán event trong Designer.
