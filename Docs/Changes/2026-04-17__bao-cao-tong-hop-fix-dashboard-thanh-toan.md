# 2026-04-17 - Bao cao tong hop dot fix Dashboard + Thanh toan

## Muc tieu
- Chot va on dinh cac thay doi quan trong ve doanh thu, thanh toan, preview hoa don, va giao dien.
- Ghi lai ket qua theo tung nhom de de truy vet va review.

## Pham vi thay doi
- Doanh thu Dashboard/Bao cao
- Man hinh Thanh toan (checkout, in lai, lich su)
- Preview hoa don trong panel ben phai
- Hien thi danh sach Khach hang
- Do net vien card va mau nen bo cuc
- In/PDF RDLC

## Tong hop fix noi bat

### 1) Doanh thu bi cong trung (180k + 24k)
- Dieu chinh query de tranh cong trung booking da co trong InvoiceDetails.
- Uu tien doanh thu theo hoa don (InvoiceDetails) khi co lien ket BookingID.
- Giu fallback booking paid cho truong hop chua xuat hoa don.

Tep lien quan:
- Services/SqlQueries.cs

### 2) Khach hang co du lieu nhung list hien trang
- Nguyen nhan: ListView o che do Details nhung chua khoi tao cot.
- Fix: Them khoi tao cot ro rang cho lstKhachHang.

Tep lien quan:
- Views/UCKhachHang.cs

### 3) Thanh toan va in lai hoa don
- Bo sung luong in lai theo ma hoa don va hoa don vua hoan tat.
- Bo sung panel lich su thanh toan va mo/in lai tu danh sach.
- Chinh lai reset state de tranh ket trang thai sau checkout.

Tep lien quan:
- Views/UCThanhToan.cs
- Views/UCThanhToan.Reprint.cs
- Views/UCThanhToan.PaymentHistory.cs
- Views/UCThanhToan.TotalsAndCheckout.cs
- Views/UCThanhToan.Courts.cs
- Services/InvoiceService.cs

### 4) Preview hoa don ben phai
- Truoc day la mock text tinh (chi hien "...").
- Da doi sang preview dong: hien san, khach, dong tinh tien, tam tinh, giam gia, tong.

Tep lien quan:
- Views/UCThanhToan.TotalsAndCheckout.cs
- Views/UCThanhToan.Designer.cs

### 5) In/PDF RDLC
- Dieu chinh chieu rong layout de tranh trang trang cuoi.
- Bo sung default/allow blank cho tham so report.
- Fallback QR -> logo neu khong co QR rieng.

Tep lien quan:
- Reports/Bill.rdlc
- Views/FrmInvoicePreview.cs

### 6) Do net vien card va nen giao dien
- Tang do tuong phan nen trang module.
- Tang do dam va do day vien card theo option "3".
- Dong bo vien khung shell (sidebar/header).

Tep lien quan:
- Services/UiTheme.cs
- Views/UCTongQuan.cs
- Views/FrmChinh.cs

## Kiem chung
- Build Debug: thanh cong.
- Push len origin/main: da hoan tat.

## Ghi chu van hanh
- Neu can rollback nhanh, co the revert theo tung commit file-level trong ngay 2026-04-17.
- Lich su commit hien tai da duoc tach theo tung tep de de audit.
