# Database workflow (script-first)

Mục tiêu: toàn bộ schema/seed ở dạng `.sql` trong repo; C# chỉ chịu trách nhiệm **tạo DB nếu chưa có**, rồi **chạy script** và **apply migrations** khi app khởi động.

## 1) Cấu hình kết nối (source of truth)

- `App.config` → connection string tên `DefaultConnection`.
- Connection string này xác định:
  - `Server` (vd: `.\SQLEXPRESS`)
  - `Database` (vd: `PickleBallDB`)
  - Auth (Integrated Security / SQL Login)

Code dùng `Services/Db.cs` để lấy connection string và tạo `SqlConnection`.

## 2) Schema script (complete)

- File: `Database/PickleBallDB_Complete.sql`
- Nguyên tắc:
  - Script **không hard-code tên DB** (`CREATE DATABASE` / `USE [PickleBallDB]` không nằm trong script).
  - Script giả định đang chạy **trong DB target** (DB được chọn từ `DefaultConnection`).

Luồng khởi động:
1. `Services/SchemaInstaller.EnsureDatabaseAndSchema()`
   - Tạo database theo `DefaultConnection` nếu chưa tồn tại.
   - Chạy `PickleBallDB_Complete.sql` vào DB đó.

## 3) Migrations (bổ sung dần)

- Thư mục: `Database/Migrations/`
- Quy ước tên file: `NNNN__Your_Description.sql`
  - Ví dụ: `0001__Add_StaffAccounts.sql`
- `0000__README.sql` chỉ là hướng dẫn và sẽ bị bỏ qua khi chạy.

Luồng khởi động (sau schema):
1. `Services/MigrationsRunner.ApplyPendingMigrations()`
   - Đảm bảo bảng `dbo.__Migrations` tồn tại.
   - Chạy các migrations chưa apply.
   - Lưu checksum; nếu migration đã apply mà nội dung bị sửa → sẽ báo lỗi (tránh drift).

## 4) Dev: Reset DB (manual)

Chỉ dùng cho dev. Vì thao tác reset DB rất dễ bấm nhầm và gây mất dữ liệu, UI `Rebuild DB (DEV)` đã được gỡ khỏi menu.

Cách reset DB khi cần test lại từ đầu:
- Drop database (theo `DefaultConnection`) trong SSMS, **hoặc** đổi `Database=...` sang tên DB khác trong `App.config`.
- Chạy lại app: app sẽ tự tạo DB + chạy schema + apply migrations.

## 5) Chạy script ở đâu?

- Bình thường: app tự chạy khi khởi động.
- Nếu muốn chạy tay: có thể mở các file trong `Database/` bằng SSMS và chạy vào đúng DB.

## 6) Notes

- Script runner có hỗ trợ tách batch theo `GO` (bao gồm `GO 10`).
- Khi thêm migration mới: **tạo file mới** trong `Database/Migrations/` thay vì sửa file migration đã apply.
