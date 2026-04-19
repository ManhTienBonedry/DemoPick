# Hướng Dẫn Chuyển Đổi Các Form Đăng Nhập (Login/Register)

Trong dự án `DemoPick`, bạn đang có cơ chế hiển thị Form đăng nhập qua Container Host có tên là `FrmAuthHost.cs`. Container này chứa cùng lúc cả *Form đăng nhập cơ bản*, *Form đăng nhập truyền thống*, và *Form đăng ký*, cho phép bạn dễ dàng chuyển đổi qua lại mà không cần phải xóa bất kỳ mã nguồn nào.

## Các Forms Hiện Đang Có:
1. `UCBasicLogin` (Form cơ bản mới nhất, chỉ gồm Username, Mật khẩu và Nút đăng nhập)
2. `UCLogin` (Form đăng nhập gốc, có bao gồm chức năng bàn phím Numpad ảo)
3. `UCRegister` (Form đăng ký tài khoản gốc)

---

## 1. Cách Đổi Sang Dùng Trang Đăng Nhập Cũ (`UCLogin`)
Hiện tại ứng dụng đang dùng `UCBasicLogin`. Nếu bạn muốn phục hồi hoặc hiển thị trang đăng nhập cũ mà có bàn phím cảm ứng và cả Nút Đăng Ký, bạn làm theo các bước sau:

1. Mở file `Views\FrmAuthHost.cs` trong Visual Studio.
2. Tìm đến hàm `ShowLogin()` (khoảng dòng 126).
3. Sửa `ShowCard(_ucBasicLogin);` thành `ShowCard(_ucLogin);`.

**Mã nguồn sau khi sửa trông như sau:**
```csharp
private void ShowLogin()
{
    // Đã thay đổi từ _ucBasicLogin sang _ucLogin
    ShowCard(_ucLogin); 
}
```
Lưu lại và chạy phần mềm, Form đăng nhập cũ sẽ xuất hiện trở lại!

---

## 2. Cách Tạo Tài Khoản Nhanh Thay Vì Phải Dùng Giao Diện Đăng Ký

Mặc định khi cơ sở dữ liệu (Database) vừa được cài đặt hoặc khởi tạo rỗng, phần mềm sẽ **TỰ ĐỘNG** tạo một tài khoản Quản trị viên (Admin) gốc nếu nó chưa tồn tại.

Theo yêu cầu, mình đã cấu hình cứng tài khoản mặc định này như sau:

- **Tên đăng nhập (Username):** `admin`
- **Email:** `admin@gmail.com`
- **Mật khẩu (Password):** `12345678`

**Khi nào tài khoản này xuất hiện?**
Khi bạn khởi động chương trình bằng môi trường Debug, nếu bảng `StaffAccounts` trong cơ sở dữ liệu trống hoàn toàn, hệ thống sẽ chèn sẵn thông tin trên. Bạn không cần phải làm gì thêm, chỉ cần vào ô Tên đăng nhập gõ `admin` hoặc `admin@gmail.com`, và nhập pass là `12345678`.

*(Đoạn code tự tạo tài khoản này nằm tại file `Services\AuthService.cs` - hàm `TrySeedAdminIfEmpty`)*.

---

## 3. Cách Thêm Đường Link "Đăng ký" vào Form Cơ Bản (Nếu Cần)

Nếu bạn vừa muốn dùng `UCBasicLogin` cho gọn, nhưng vẫn muốn bấm vào đâu đó để sang được màn hình tạo tài khoản của bạn bè (`UCRegister`):
1. Bên trong `UCBasicLogin.Designer.cs`, kéo thả một `LinkLabel` tên là `lblRegister` (hoặc Label bình thường).
2. Tạo sự kiện Click cho Label đó:
   ```csharp
   lblRegister.Click += (s, e) => {
       RequestRegister?.Invoke(this, EventArgs.Empty);
   };
   ```
3. Khai báo sự kiện ở `UCBasicLogin.cs`:
   ```csharp
   public event EventHandler RequestRegister;
   ```
4. Ở file `FrmAuthHost.cs` thêm dòng:
   ```csharp
   _ucBasicLogin.RequestRegister += (s, e) => ShowRegister();
   ```

Như vậy bạn sẽ có cả 2 thế giới: một Form rất basic gọn nhẹ và vẫn có nút để qua trang đăng ký khi cần!
