# Timeline Zoom (+/-) – Sơ đồ đặt lịch

## Mục tiêu
- Thêm nút `-` và `+` cạnh khu vực chọn ngày ở trang **Sơ đồ đặt lịch** để người dùng **phóng to/thu nhỏ độ rộng theo giờ** (timeline) cho dễ nhìn.
- Khắc phục tình trạng nhãn giờ ở mép phải (ví dụ **23:00**) bị che/cắt khi panel xuất hiện **scrollbar dọc**.

## UX / Cách dùng
- `+` (**Zoom in**): tăng độ rộng mỗi giờ → timeline rộng hơn → xuất hiện **scroll ngang** nếu cần.
- `-` (**Zoom out**): giảm dần về mức mặc định (fit-to-width).
- Mức nhỏ nhất là **mặc định vừa khít** (không “nén” nhỏ hơn fit-to-width để tránh khoảng trống và lệch layout).

## Thay đổi kỹ thuật

### 1) Thêm 2 button zoom trên ControlBar
- Thêm `btnZoomOut` và `btnZoomIn` (Sunny.UI) vào `pnlControlBar`, đặt ngay bên phải `dateFilter`.

File:
- Views/UCDatLich.Designer.cs

### 2) Zoom làm thay đổi bề rộng giờ (hourWidth)
- Thêm biến `_zoom` và các hằng số:
  - `ZoomMin = 1.0f` (fit-to-width)
  - `ZoomMax = 2.5f`
  - `ZoomStep = 0.25f`
- Khi zoom thay đổi → gọi `RefreshTimeline()` để tính lại `pnlCanvas.Width`.
- Công thức chính:
  - `baseHourWidth = (visibleWidth - CourtColWidth) / GridHoursToDraw`
  - `hourWidth = baseHourWidth * _zoom`
  - `requiredWidth = max(visibleWidth, CourtColWidth + hoursToDraw * hourWidth)`

File:
- Views/UCDatLich.cs

### 3) Fix “23:00 bị che” khi có scrollbar dọc
- Nguyên nhân: khi scrollbar dọc xuất hiện, `ClientSize.Width` giảm nhưng đôi khi logic resize/paint chưa trừ phần scrollbar chuẩn → nhãn giờ cuối bị vẽ vào vùng bị scrollbar che.
- Cách sửa: dự đoán việc xuất hiện scrollbar dọc bằng cách so `requiredHeight` với `pnlTimelineContainer.ClientSize.Height`, nếu chắc chắn có scrollbar → trừ `SystemInformation.VerticalScrollBarWidth` ngay khi tính `visibleWidth`.

File:
- Views/UCDatLich.cs

## Verify
- Build `Release` đã chạy OK.
- Nếu build `Debug` báo lỗi copy exe: thường do `bin\Debug\DemoPick.exe` đang chạy (file bị lock). Đóng app rồi build lại.
