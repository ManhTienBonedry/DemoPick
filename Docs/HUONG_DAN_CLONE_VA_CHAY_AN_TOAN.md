# Huong Dan Clone Va Chay Du An (Ban An Toan)

Tai lieu nay dung de gui cho ban be clone va chay nhanh du an DemoPick tren may Windows.

## 1. Yeu Cau Moi Truong

- Windows 10/11
- Visual Studio 2022 (hoac Build Tools co MSBuild)
- SQL Server Express (khuyen nghi `SQLEXPRESS`)
- .NET Framework 4.8 Developer Pack

## 2. Clone Du An

```powershell
git clone <YOUR_REPO_URL>
cd DemoPick
```

## 3. Cau Hinh Ket Noi Database

Mac dinh du an dung connection string trong App.config:

- Server: `\\.\\SQLEXPRESS`
- Database: `PickleProDB`
- Integrated Security: `True`

Co the override bang bien moi truong (khuyen nghi cho moi truong khac nhau):

```powershell
setx DEMOPICK_CONNECTION_STRING "Server=.\\SQLEXPRESS;Database=PickleProDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
```

Sau khi set `setx`, dong mo lai terminal/VS de bien moi truong co hieu luc.

## 4. Restore Va Build

Du an khong commit `nuget.exe` vao repo. Cai NuGet CLI tren may local (1 lan):

```powershell
winget install -e --id Microsoft.NuGet
```

Sau do restore:

```powershell
nuget restore .\\DemoPick.sln -NonInteractive
```

Sau do build:

```powershell
"D:\\vstudio\\Join\\MSBuild\\Current\\Bin\\MSBuild.exe" .\\DemoPick.sln /p:Configuration=Debug /v:m
```

## 5. Chay Ung Dung

Chay file EXE sau build:

```powershell
.\\bin\\Debug\\DemoPick.exe
```

Lan chay dau, app se:

- Tao database neu chua co
- Chay script schema
- Chay migrations (bao gom migration seed 4 mon thue dung cu mac dinh)

4 mon mac dinh o phan thue dung cu:

- Thue vot - 40.000d
- Bong tap (ro) - 40.000d
- May ban bong - 80.000d
- Nhat bong - 40.000d

## 6. Dang Nhap Admin Luc Khoi Tao (Debug)

Trong build DEBUG, neu database StaffAccounts trong, he thong co the seed tai khoan admin.

Khuyen nghi dat mat khau bootstrap chu dong:

```powershell
setx DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD "<MatKhauManhCuaBan>"
```

Neu khong dat bien nay, app co the tao mat khau ngau nhien.

## 7. Checklist Bao Mat Truoc Khi Chia Se Repo

### 7.1 Da harden trong code

- PBKDF2 da dung SHA256 cho hash moi.
- Co co che tuong thich hash cu SHA1 trong luc dang nhap, va tu nang cap hash len SHA256 sau khi dang nhap thanh cong.
- Connection string mac dinh da bat `Encrypt=True`.

### 7.2 Khuyen nghi van hanh tiep (muc 4/5)

- Khong commit file nhi phan cong cu vao repo dai han (`nuget.exe`).
- Neu chua the bo ngay (do workflow), can:
  - Chot version ro rang
  - Tai tu nguon chinh chu
  - Kiem tra checksum/chu ky dinh ky
- Khong chia se screenshot co thong tin bootstrap admin.

## 8. Su Co Thuong Gap

- Loi ket noi SQL:
  - Kiem tra SQL Server service dang chay
  - Kiem tra instance name (`SQLEXPRESS`)
  - Kiem tra bien `DEMOPICK_CONNECTION_STRING`

- Build fail do package:
  - Chay lai nuget restore
  - Kiem tra folder `packages` da duoc tai day du

- Dang nhap khong duoc ngay lan dau:
  - Dat lai `DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD`
  - Xoa DB test va chay lai trong moi truong dev

## 9. Khuyen Nghi Khi Day Len Moi Truong Team

- Moi nguoi dung 1 connection string rieng qua bien moi truong
- Tach tai khoan SQL theo moi truong (dev/test)
- Khong dung chung mat khau bootstrap
- Dinh ky doi mat khau admin
