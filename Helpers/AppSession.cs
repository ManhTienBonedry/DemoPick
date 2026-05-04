using DemoPick.Helpers;
using DemoPick.Data;
using DemoPick.Models;

namespace DemoPick.Helpers
{
    internal static class AppSession
    {
        internal static AuthUser CurrentUser { get; private set; }

        // Cap nhat phien dang nhap khi nguoi dung dang nhap hoac dang xuat.
        internal static void SignIn(AuthUser user)
        {
            CurrentUser = user;
        }

        // Cap nhat phien dang nhap khi nguoi dung dang nhap hoac dang xuat.
        internal static void SignOut()
        {
            CurrentUser = null;
        }

        // Kiem tra dieu kien Is In Role va tra ve ket qua dung/sai cho luong xu ly.
        internal static bool IsInRole(string role)
        {
            if (CurrentUser == null) return false;
            if (string.IsNullOrWhiteSpace(role)) return false;

            return string.Equals(CurrentUser.Role ?? "", role, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}


