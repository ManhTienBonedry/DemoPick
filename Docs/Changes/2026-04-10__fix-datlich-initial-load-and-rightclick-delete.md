# 2026-04-10 - Fix DatLich initial load and right-click delete

## Muc tieu
- Sua loi man hinh Dat lich bi trang khi vua dang nhap (chi hien thi dung sau khi bam chuyen ngay).
- Sua hanh vi chuot phai tren timeline khong mo nham luong doi ca.
- Bo sung thao tac xoa booking bang chuot phai tren block booking.

## Root cause
1. Tai du lieu lan dau duoc goi ngay trong constructor cua UCDatLich.
2. Trong luong async, neu control chua co handle (`IsHandleCreated == false`) thi callback bi return som.
3. Do callback bi bo qua, cache courts/bookings khong duoc nap va canvas ve trang cho den khi co thao tac reload khac (vi du: chuyen ngay).
4. `PnlCanvas_MouseDoubleClick` khong kiem tra nut chuot, nen right double-click van chay luong mo form doi ca.

## File da thay doi
- Views/UCDatLich.cs
- Views/UCDatLich.LayoutAndInteractions.cs
- Controllers/BookingController.cs

## Noi dung thay doi
### 1) UCDatLich.cs
- Khong goi `ReloadTimelineAsync(true)` ngay lap tuc neu control chua co handle.
- Dang ky `HandleCreated` one-time de reload lan dau dung thoi diem.

### 2) UCDatLich.LayoutAndInteractions.cs
- Trong right-click:
  - Uu tien detect booking duoi con tro va mo context menu booking.
  - Neu khong trung booking thi moi fallback sang context menu cot san.
- Them `TryShowBookingContextMenu(...)`:
  - Menu item: "Xoa dat san".
  - Xac nhan truoc khi xoa.
  - Goi `_controller.CancelBooking(booking.BookingID)`.
  - Reload timeline sau khi xoa.
- Trong `PnlCanvas_MouseDoubleClick(...)`:
  - Them guard chi xu ly khi `e.Button == MouseButtons.Left`.

### 3) BookingController.cs
- Them method `CancelBooking(int bookingId)`:
  - Validate booking ton tai.
  - Khong cho xoa booking da thanh toan.
  - Neu da Cancelled thi coi nhu no-op.
  - Soft-delete bang cap nhat `Status = 'Cancelled'`.

## Anh huong va tuong thich
- Khong thay doi schema DB.
- Khong tac dong den luong checkout/huy san khac ngoai hanh vi xoa booking tu Dat lich.
- Giu logic soft-delete thong nhat voi cac query dang loc `Status != 'Cancelled'`.

## Cach test nhanh
1. Dang nhap vao app va mo tab "So do & Dat lich":
- Ky vong: timeline load ngay, khong trang.
2. Right-click tren block booking:
- Ky vong: hien menu "Xoa dat san".
3. Chon xoa va xac nhan:
- Ky vong: booking bien mat khoi timeline sau reload.
4. Right double-click tren booking:
- Ky vong: khong mo form doi ca.
5. Left double-click tren booking:
- Ky vong: van mo form doi ca nhu cu.

## Rollback
- Revert 3 file da thay doi neu can quay lai hanh vi cu.
