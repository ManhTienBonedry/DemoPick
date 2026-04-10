# 2026-04-10 - Fix Designer khong mo duoc form (global design-mode guard)

## Muc tieu
- Khac phuc tinh trang mo WinForms Designer bi loi hang loat (khong mo duoc UI cua nhieu form/usercontrol).
- Tang do on dinh cho moi constructor dang dung `DesignModeUtil.IsDesignMode(...)`.

## Root cause (kha nang cao)
- Designer cua Visual Studio co the khong set day du `Site.DesignMode`/`LicenseManager` o mot so thoi diem.
- Khi check design mode tra ve sai, constructor co the tiep tuc chay logic runtime (DB/service/session), lam designer bi fail va hien loi chung.

## File da thay doi
- Services/DesignModeUtil.cs

## Noi dung thay doi
- Bo sung detect tien trinh host designer:
  - Neu process name la `devenv` => tra ve `true` ngay.
- Bo sung check `control.DesignMode` (best-effort).
- Bo sung fallback theo `Assembly.GetEntryAssembly()`:
  - Neu `null` (thuong gap o context design host) => coi la design-time.
- Giu lai check cu `LicenseManager.UsageMode` va `control.Site.DesignMode`.

## Tac dong
- Tat ca form/usercontrol dang goi `DesignModeUtil.IsDesignMode(...)` se duoc bao ve tot hon trong Designer.
- Khong anh huong runtime thong thuong khi chay app `DemoPick.exe`.

## Cach verify
1. Mo lai Visual Studio.
2. Build solution Debug.
3. Mo Designer cua cac man:
   - FrmChinh
   - UCDatLich
   - UCBaoCao
   - UCThanhToan
4. Ky vong: Designer mo duoc UI, khong con thong bao "base class Form could not be loaded".

## Rollback
- Revert file `Services/DesignModeUtil.cs` neu can.
