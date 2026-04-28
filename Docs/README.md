# DemoPick – Click/Event Wiring Map

Tài liệu này dùng để:
- Theo dõi nhanh: control nào đang được gán click/event, gọi handler nào.
- Tránh lỗi **gán event 2 lần** (Designer + code-behind) khiến handler chạy 2 lần.

## Tài liệu liên quan

- Database workflow (script-first, migrations): `DB-WORKFLOW.md`
- Checklist chống double-wiring: `CLICK-WIRING-CHECKLIST.md`

## Quy tắc quan trọng (tránh gán 2 lần)

Trong WinForms, event là **danh sách delegate**. Nếu bạn gán cùng handler 2 lần, nó sẽ chạy 2 lần.

- **OK** (chọn 1 cách):
  - Gán trong Designer (`*.Designer.cs`) **hoặc**
  - Gán trong code-behind (constructor sau `InitializeComponent()`).
- **Không nên**: vừa gán trong Designer, vừa gán trong code-behind cho cùng control/handler.

Mẹo an toàn khi gán bằng code-behind (chống gán trùng):
```csharp
btnSomething.Click -= BtnSomething_Click;
btnSomething.Click += BtnSomething_Click;
```

Lưu ý riêng với menu trái `FrmChinh`: hàm `BindClick()` đã tự gán click cho **UIPanel và tất cả control con** (label/icon trong panel). Nếu bạn tiếp tục gán click vào label con trong Designer nữa, có thể bị chạy lặp.

## Cơ chế chạy ứng dụng (Login → Main)

- App khởi động theo luồng:
  - `Program.cs` gọi `SchemaInstaller.EnsureDatabaseAndSchema()` để tự tạo DB/bảng nếu thiếu.
  - Mở `FrmAuthHost`.
  - Login OK → mở `FrmChinh`.
  - Logout → quay lại `FrmAuthHost`.

Xem code:
- `../Program.cs`

## Cơ chế điều hướng chính (trong `FrmChinh`)

- `FrmChinh.InitModules()` tạo các module (`UCTongQuan`, `UCDatLich`, `UCBanHang`, …) và gọi `BindClick()` để gán click cho menu.
- `BindClick()` → gọi `SwitchModule()` để thay nội dung `pnlContent`.

Xem code:
- `../Views/FrmChinh.cs`

## Danh sách event đã gán (theo màn)

### 1) Form chính: Sidebar / Header

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `FrmChinh` | `btnNavDashboard` + label con | `Click` | `BindClick(... tongQuan ...)` | Mở Tổng quan |
| `FrmChinh` | `btnNavDatLich` + label con | `Click` | `BindClick(... datLich ...)` | Mở Sơ đồ & Đặt lịch |
| `FrmChinh` | `btnNavBanHang` + label con | `Click` | `BindClick(... banHang ...)` | Mở POS |
| `FrmChinh` | `btnNavKhachHang` + label con | `Click` | `BindClick(... khachHang ...)` | Mở Khách hàng |
| `FrmChinh` | `btnNavKhoHang` + label con | `Click` | `BindClick(... khoHang ...)` | Mở Kho hàng |
| `FrmChinh` | `btnNavBaoCao` + label con | `Click` | `BindClick(... baoCao ...)` | Mở Báo cáo |
| `FrmChinh` | `pnlLogo` + control con | `Click` | `logoClick => SwitchModule(...)` | Về Dashboard |
| `FrmChinh` | `pnlAdminAvatar` + control con | `Click` | `UserMenu_Click` | Mở menu user |

### 2) Báo cáo

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCBaoCao` | `btnApplyFilter` | `Click` | `ApplyFilterAsync()` | Lọc theo khoảng thời gian |
| `UCBaoCao` | `lblTopCourtsViewAll` | `Click` | `NavigateToDatLich()` | Điều hướng sang Đặt lịch |
| `UCBaoCao` | `lstTopCourts` | `DoubleClick` | `NavigateToDatLich()` | Điều hướng sang Đặt lịch |

### 3) Đặt lịch

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCDatLich` | `btnPrevDay` | `Click` | lambda | Lùi ngày |
| `UCDatLich` | `btnNextDay` | `Click` | lambda | Tiến ngày |
| `UCDatLich` | `court row trên timeline` | `Click` | click 1 lần để arm, click lần 2 mở `FrmDatSanCoDinh(BookingMode.Quick)` | Đặt sân chơi |
| `UCDatLich` | `btnDoiCa` | `Click` | `OpenRescheduleForSelected()` | Đổi ca booking đã chọn |

### 4) POS (Bán hàng)

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCBanHang` | `btnAddProduct` | `Click` | `BtnAddProduct_Click` | Mở form thêm SP |
| `UCBanHang` | `btnApplyDiscount` | `Click` | `BtnApplyDiscount_Click` | Áp dụng giảm giá |
| `UCBanHang` | `btnCheckout` | `Click` | `BtnCheckout_Click` | Thanh toán |

### 5) Khách hàng

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCKhachHang` | `lblTabAll` | `Click` | `FilterList("Tất cả", ...)` | Lọc danh sách |
| `UCKhachHang` | `lblTabVip` | `Click` | `FilterList("VIP", ...)` | Lọc danh sách |
| `UCKhachHang` | `lblTabNew` | `Click` | `FilterList("Mới", ...)` | Lọc danh sách |

### 6) Kho hàng

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCKhoHang` | `btnThemSP` | `Click` | `BtnThemSP_Click` | Mở form nhập hàng |

### 7) Login / Register / User menu

| Màn | Control | Event | Handler / Logic | Mục đích |
|---|---|---|---|---|
| `UCLogin` | `btnLogin` | `Click` | `btnLogin_Click` | Đăng nhập (SQL) |
| `UCLogin` | `btnRegister` | `Click` | `btnRegister_Click` | Mở đăng ký |
| `UCLogin` | `lblRegisterNow` | `Click` | `lblRegisterNow_Click` | Mở đăng ký |
| `UCLogin` | `btnClose` (Label X) | `Click` | `btnClose_Click` | Thoát / hủy |
| `UCRegister` | `btnRegister` | `Click` | `btnRegister_Click` | Đăng ký (SQL) |
| `UCRegister` | `lblLoginNow` | `Click` | `lblLoginNow_Click` | Quay lại |
| `UCRegister` | `btnClose` (Label X) | `Click` | `btnClose_Click` | Quay lại đăng nhập |
| `FrmUserMenu` | `btnDoiMatKhau` | `Click` | `btnDoiMatKhau_Click` | Đổi mật khẩu |
| `FrmUserMenu` | `btnDangXuat` | `Click` | `btnDangXuat_Click` | Đăng xuất |
| `FrmUserMenu` | `btnThoat` | `Click` | `btnThoat_Click` | Thoát app |
