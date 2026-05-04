# Giải thích chi tiết các tệp trong dự án DemoPick

Đây là tài liệu mô tả chi tiết về cấu trúc và chức năng của từng tệp trong dự án.

## **Thư mục gốc (`/`)**

Các tệp cấu hình và khởi động chính của ứng dụng.

- **`App.config`**: Tệp cấu hình XML của ứng dụng .NET. Chứa các cài đặt như chuỗi kết nối cơ sở dữ liệu (connection strings), và các cài đặt ứng dụng khác.
- **`DemoPick.csproj`**: Tệp dự án MSBuild. Nó định nghĩa cách dự án được xây dựng, bao gồm danh sách các tệp mã nguồn, các tài nguyên, và các gói phụ thuộc (NuGet packages).
- **`DemoPick.sln`**: Tệp "Solution" của Visual Studio. Nó dùng để tổ chức và quản lý một hoặc nhiều dự án liên quan.
- **`HUONG_DAN_GUI_DANG_NHAP.md`**: Tệp Markdown chứa hướng dẫn về giao diện đăng nhập.
- **`NuGet.Config`**: Tệp cấu hình cho NuGet, trình quản lý gói của .NET. Có thể dùng để chỉ định các nguồn (repository) gói tùy chỉnh.
- **`packages.config`**: Tệp XML kiểu cũ để quản lý các gói NuGet đã cài đặt trong dự án.
- **`Program.cs`**: Điểm vào (entry point) của ứng dụng. Chứa phương thức `static void Main()`, nơi chương trình bắt đầu thực thi. Thường thì nó sẽ khởi tạo và hiển thị cửa sổ chính (form).
- **`README.md`**: Tệp Markdown chứa thông tin giới thiệu chung về dự án.

---

## **`/Controllers/`**

Các lớp điều phối, nhận yêu cầu từ `View` và gọi các `Service` tương ứng để xử lý logic.

- **`BookingController.cs`**: Điều phối các hoạt động liên quan đến việc đặt sân (tạo, sửa, hủy lịch).
- **`CustomerController.cs`**: Điều phối các tác vụ quản lý khách hàng (thêm, sửa, tìm kiếm, xem lịch sử).
- **`DashboardController.cs`**: Chịu trách nhiệm thu thập và chuẩn bị dữ liệu để hiển thị trên bảng điều khiển (dashboard), ví dụ: doanh thu, số lượng khách.
- **`InventoryController.cs`**: Điều phối các hoạt động quản lý kho (sản phẩm, nước uống), như nhập hàng, xuất hàng, kiểm kê.
- **`ReportController.cs`**: Điều phối việc tạo và hiển thị các loại báo cáo khác nhau (báo cáo doanh thu, báo cáo tồn kho).
- **`ThanhToanController.cs`**: Điều phối quy trình thanh toán cho các hóa đơn, lịch đặt sân.

---

## **`/Data/`**

Lớp truy cập dữ liệu, chịu trách nhiệm giao tiếp trực tiếp với cơ sở dữ liệu.

- **`DatabaseHelper.cs`**: Cung cấp các phương thức tiện ích để thực thi các câu lệnh SQL, chuyển đổi dữ liệu từ `DataReader` sang các đối tượng `Model`.
- **`DatabaseMaintenanceService.cs`**: Chứa các chức năng để bảo trì cơ sở dữ liệu, ví dụ như sao lưu (backup) và phục hồi (restore).
- **`Db.cs`**: Lớp trung tâm để quản lý kết nối cơ sở dữ liệu (`SqlConnection`). Cung cấp các phương thức để mở, đóng kết nối và thực thi các lệnh.
- **`DbDiagnostics.cs`**: Cung cấp các công cụ để chẩn đoán hiệu suất cơ sở dữ liệu, ví dụ như ghi log (logging) các truy vấn chạy chậm.
- **`MigrationsRunner.cs`**: Chịu trách nhiệm chạy các kịch bản di chuyển (migration scripts) để cập nhật cấu trúc cơ sở dữ liệu một cách tự động.
- **`SchemaInstaller.cs`**: Chịu trách nhiệm cài đặt cấu trúc (schema) ban đầu cho cơ sở dữ liệu.
- **`SqlQueries.cs`**: Một lớp tĩnh chứa các chuỗi truy vấn SQL dưới dạng các hằng số. Giúp tránh việc viết truy vấn trực tiếp trong mã logic, dễ quản lý và tái sử dụng.
- **`SqlScriptRunner.cs`**: Một tiện ích để đọc và thực thi nội dung của một tệp kịch bản `.sql`.

---

## **`/Database/`**

Chứa các kịch bản SQL để thiết lập và quản lý cơ sở dữ liệu.

- **`PickleBallDB_Complete.sql`**: Kịch bản SQL chính để tạo toàn bộ cơ sở dữ liệu, bao gồm các bảng, view, stored procedure từ đầu.
- **`TesterData_Seed.sql`**: Kịch bản SQL để chèn dữ liệu mẫu hoặc dữ liệu thử nghiệm vào cơ sở dữ liệu.
- **`/Legacy/`**: Thư mục có thể chứa các kịch bản SQL cũ hoặc các phiên bản trước của cơ sở dữ liệu.
- **`/Migrations/`**: Thư mục chứa các kịch bản di chuyển, mỗi tệp tương ứng với một sự thay đổi nhỏ trong cấu trúc cơ sở dữ liệu.

---

## **`/Helpers/`**

Chứa các lớp tiện ích nhỏ, thực hiện các nhiệm vụ cụ thể và có thể tái sử dụng ở nhiều nơi.

- **`AppConstants.cs`**: Định nghĩa các hằng số được sử dụng trong toàn bộ ứng dụng.
- **`AppSession.cs`**: Quản lý trạng thái phiên làm việc của người dùng, ví dụ như lưu thông tin của người dùng đang đăng nhập.
- **`AuthFeatures.cs`**: Có thể chứa các cờ (flags) để bật/tắt các tính năng liên quan đến xác thực.
- **`AuthLoginAttemptTracker.cs`**: Theo dõi số lần đăng nhập thất bại để chống lại các cuộc tấn công brute-force.
- **`AuthPasswordCrypto.cs`**: Chứa các hàm để băm (hash) và xác minh mật khẩu, đảm bảo mật khẩu không được lưu dưới dạng văn bản thuần.
- **`DesignModeUtil.cs`**: Cung cấp các phương thức để kiểm tra xem ứng dụng đang chạy ở chế độ thiết kế (design mode) trong Visual Studio hay không.
- **`InventoryTransactionFormatter.cs`**: Định dạng thông tin giao dịch kho để hiển thị hoặc ghi log.
- **`MembershipTierHelper.cs`**: Giúp xử lý logic liên quan đến các hạng thành viên của khách hàng.
- **`NativeMethods.cs`**: Dùng để gọi các hàm từ API của hệ điều hành Windows (P/Invoke).
- **`PhoneNumberValidator.cs`**: Kiểm tra tính hợp lệ của số điện thoại.
- **`PosCheckoutLogFormatter.cs`**: Định dạng log cho quá trình thanh toán tại quầy (Point of Sale).
- **`PosGuestInfoParser.cs`**: Phân tích thông tin khách vãng lai từ một chuỗi hoặc định dạng nào đó.
- **`PosInventoryValidator.cs`**: Kiểm tra tính hợp lệ của các mặt hàng trong kho khi bán hàng.
- **`PosInvoiceWriter.cs`**: Ghi thông tin hóa đơn ra tệp hoặc máy in.
- **`PosMemberResolver.cs`**: Tìm kiếm và xác định thông tin thành viên tại quầy bán hàng.
- **`PriceCalculator.cs`**: Tính toán giá tiền cho các dịch vụ, sản phẩm, có thể bao gồm cả logic khuyến mãi.
- **`UiTheme.cs`**: Quản lý các thiết lập về giao diện người dùng (màu sắc, font chữ) để đảm bảo tính nhất quán.

---

## **`/Models/`**

Định nghĩa các lớp đối tượng dữ liệu (Plain Old CLR Object - POCO). Chúng chỉ chứa các thuộc tính để lưu trữ dữ liệu.

- **`AuthUser.cs`**: Đại diện cho một người dùng trong hệ thống (Id, Tên đăng nhập, Mật khẩu đã băm, Vai trò).
- **`BookingModels.cs`**: Chứa các lớp liên quan đến việc đặt sân như `Booking` (một lượt đặt), `Court` (một sân), `TimeSlot` (một khung giờ).
- **`CartLine.cs`**: Đại diện cho một dòng hàng trong giỏ hàng hoặc hóa đơn (Sản phẩm, Số lượng, Đơn giá).
- **`CustomerModels.cs`**: Đại diện cho thông tin của một khách hàng (Id, Tên, Số điện thoại, Hạng thành viên).
- **`DashboardModels.cs`**: Chứa các lớp dữ liệu được thiết kế riêng để phục vụ cho việc hiển thị trên dashboard.
- **`InventoryModels.cs`**: Chứa các lớp như `Product` (Sản phẩm), `Stock` (Tồn kho).
- **`ReportModels.cs`**: Chứa các lớp cấu trúc dữ liệu cho các báo cáo.

---

## **`/Properties/`**

Chứa các tệp siêu dữ liệu và tài nguyên của dự án.

- **`AssemblyInfo.cs`**: Chứa thông tin về assembly (phiên bản, tên công ty, mô tả...).
- **`Resources.resx`**: Tệp tài nguyên mặc định của dự án. Chứa hình ảnh, biểu tượng, chuỗi văn bản có thể được sử dụng trong ứng dụng.
- **`Resources.Designer.cs`**: Tệp được tạo tự động, cung cấp quyền truy cập vào các tài nguyên trong `Resources.resx` một cách tường minh (strongly-typed).
- **`Settings.settings`**: Tệp để định nghĩa các cài đặt của ứng dụng.
- **`Settings.Designer.cs`**: Tệp được tạo tự động để cung cấp quyền truy cập vào các cài đặt trong `Settings.settings`.

---

## **`/Reports/`**

Chứa các tệp định nghĩa mẫu báo cáo.

- **`Bill.rdlc`**: Tệp định nghĩa báo cáo (Report Definition Language Client-side) cho việc in hóa đơn. Bạn có thể mở và chỉnh sửa nó bằng trình thiết kế báo cáo của Visual Studio.

---

## **`/Services/`**

Lớp logic nghiệp vụ (Business Logic Layer). Đây là "bộ não" của ứng dụng, nơi xử lý tất cả các quy tắc và quy trình nghiệp vụ.

- **`AuthService.cs`**: Chứa logic để xác thực người dùng (đăng nhập, đăng xuất), kiểm tra quyền hạn.
- **`BookingCourtCommandService.cs`**: Xử lý các lệnh **thay đổi** dữ liệu liên quan đến việc đặt sân (tạo, cập nhật, hủy).
- **`BookingCourtQueryService.cs`**: Xử lý các truy vấn **đọc** dữ liệu liên quan đến việc đặt sân.
- **`BookingMemberCleanupService.cs`**: Dịch vụ chạy nền để dọn dẹp dữ liệu cũ hoặc không hợp lệ liên quan đến việc đặt sân của thành viên.
- **`BookingMemberService.cs`**: Xử lý các nghiệp vụ đặt sân dành riêng cho thành viên.
- **`BookingQueryService.cs`**: Cung cấp các phương thức truy vấn chung cho dữ liệu đặt sân.
- **`BookingWriteService.cs`**: Cung cấp các phương thức ghi chung cho dữ liệu đặt sân.
- **`CustomerService.cs`**: Chứa logic nghiệp vụ liên quan đến khách hàng (ví dụ: nâng hạng thành viên, tính điểm tích lũy).
- **`DashboardService.cs`**: Chứa logic để tính toán và tổng hợp dữ liệu cho dashboard.
- **`InventoryService.cs`**: Chứa logic quản lý kho (tính toán tồn kho, đề xuất nhập hàng).
- **`InvoiceService.cs`**: Chứa logic để tạo và xử lý hóa đơn.
- **`PosBookingPaymentStateService.cs`**: Quản lý trạng thái thanh toán của các lượt đặt sân tại quầy POS.
- **`PosService.cs`**: Chứa logic nghiệp vụ cho các hoạt động tại quầy bán hàng (Point of Sale).
- **`ReportService.cs`**: Chứa logic để tạo dữ liệu cho các báo cáo.

---

## **`/Views/`**

Chứa các tệp giao diện người dùng (Windows Forms).

- **`FrmAuthHost.cs`**: Form chính chứa các giao diện người dùng liên quan đến xác thực (như panel đăng nhập, đăng ký).
- **`FrmChinh.cs`**: Form chính của ứng dụng, hiển thị sau khi người dùng đăng nhập thành công.
- **`FrmDatSanCoDinh.cs`**: Form cho chức năng đặt sân cố định. Logic của form này được chia thành nhiều tệp con (`.Booking.cs`, `.Init.cs`, `.WindowChrome.cs`) bằng cách sử dụng `partial class` để dễ quản lý.
- **`FrmDoiCaBooking.cs`**: Form để xử lý việc đổi ca cho một lượt đặt sân.
- **`FrmDoiMatKhau.cs`**: Form cho phép người dùng thay đổi mật khẩu của họ.
- **`FrmInvoicePreview.cs`**: Form để xem trước hóa đơn trước khi in.
- **`FrmThemSP.cs`**: Form để thêm một sản phẩm mới vào kho.
- **`FrmUserMenu.cs`**: Form hiển thị menu người dùng (ví dụ: đổi mật khẩu, đăng xuất).
- **`FrmXoaSP.cs`**: Form để xóa một sản phẩm khỏi kho.
- **`UCBanHang.cs`**: User Control (UC) cho giao diện bán hàng chính. Logic được chia nhỏ thành các tệp partial: `Courts.cs` (quản lý hiển thị sân), `Catalog.cs` (hiển thị danh mục sản phẩm), `Cart.cs` (quản lý giỏ hàng).
- **`UCBaoCao.cs`**: User Control để hiển thị các loại báo cáo.
- **`UCBasicLogin.cs`**: Một User Control đăng nhập cơ bản.
- **`UCCategoryChip.cs`**: Một User Control nhỏ gọn để hiển thị một danh mục dưới dạng "chip".
- **`UCDatLich.cs`**: User Control chính cho chức năng đặt lịch. Logic phức tạp được chia thành các tệp partial: `Data.cs` (xử lý dữ liệu), `LayoutAndInteractions.cs` (xử lý giao diện và tương tác), `Rendering.cs` (xử lý vẽ giao diện).
- **`UCKhoHang.cs`**: User Control để quản lý giao diện kho hàng.
- **`UCKhachHang.cs`**: User Control để quản lý giao diện khách hàng.
- **`UCLogin.cs`**: User Control cho chức năng đăng nhập.
- **`UCRegister.cs`**: User Control cho chức năng đăng ký tài khoản mới.
- **`UCThanhToan.cs`**: User Control cho giao diện thanh toán. Logic được chia nhỏ: `Courts.cs`, `Customer.cs`, `PaymentHistory.cs`, `Reprint.cs`, `TotalsAndCheckout.cs`.
- **`UCTongQuan.cs`**: User Control cho màn hình tổng quan (dashboard).
- **`UCAuditLog.cs`**: User Control để xem nhật ký hoạt động của hệ thống.
- **`/Controls/`**: Thư mục con chứa các User Control có thể tái sử dụng ở nhiều nơi.
    - **`UCDateRangeFilter.cs`**: Một control cho phép người dùng chọn một khoảng ngày để lọc dữ liệu.
    - **`UCInvoiceReprintPanel.cs`**: Một panel cho phép tìm và in lại hóa đơn.
    - **`UCPaymentHistoryPanel.cs`**: Một panel hiển thị lịch sử thanh toán.
- **`*.Designer.cs`**: Tệp chứa mã do trình thiết kế của Visual Studio tạo ra (vị trí, kích thước của các control). Bạn không nên chỉnh sửa tệp này bằng tay.
- **`*.resx`**: Tệp tài nguyên dành riêng cho một Form hoặc User Control, chứa các chuỗi, hình ảnh... được sử dụng trên đó.

---

## **`/Resources/`**

Chứa các tệp tài nguyên chung như hình ảnh, biểu tượng được sử dụng trong toàn bộ ứng dụng.

- Các tệp `.png`, `.jpg`: Là các tệp hình ảnh được sử dụng cho các nút, nền, hoặc trong các báo cáo.

---

## **`/bin/` và `/obj/`**

Đây là các thư mục do Visual Studio tự động tạo ra trong quá trình xây dựng (build) dự án.

- **`/obj/`**: Chứa các tệp đối tượng trung gian được tạo ra trong quá trình biên dịch.
- **`/bin/`**: Chứa kết quả cuối cùng của quá trình build.
    - **`/bin/Debug/`**: Chứa các tệp thực thi (`.exe`), thư viện (`.dll`) và các tệp khác cho phiên bản gỡ lỗi (debug).
    - **`/bin/Release/`**: Chứa các tệp đã được tối ưu hóa cho phiên bản phát hành chính thức.

Bạn thường không cần phải chỉnh sửa trực tiếp các tệp trong hai thư mục này.
