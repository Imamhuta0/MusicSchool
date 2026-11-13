using Ktpt.MusicSchool.Data;
using Ktpt.MusicSchool.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Services;

public static class Session
{
    public static User? CurrentUser { get; private set; }

    public static string RoleName => CurrentUser?.Role?.RoleName ?? "";

    public static bool IsGuest => CurrentUser == null;
    public static bool IsStudent => RoleName == "Ученик";
    public static bool IsTeacher => RoleName == "Преподаватель";
    public static bool IsAdmin => RoleName == "Администратор";
    public static bool IsStaff => IsTeacher || IsAdmin;

    public static string Caption =>
        CurrentUser == null ? "Гость" : $"{CurrentUser.FullName} · {RoleName}";

    public static async Task<bool> LoginAsync(string login, string password)
    {
        using var db = Db.Create();
        User? user = await db.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Login == login && u.PasswordHash == password);

        if (user == null) return false;
        CurrentUser = user;
        return true;
    }

    public static void Logout() => CurrentUser = null;

    public static void RequireStaff()
    {
        if (!IsStaff) throw new AppException("Недостаточно прав: нужна роль преподавателя или администратора.");
    }

    public static void RequireAdmin()
    {
        if (!IsAdmin) throw new AppException("Недостаточно прав: нужна роль администратора.");
    }

    public static async Task<List<User>> GetUsersAsync()
    {
        using var db = Db.Create();
        return await db.Users
            .Include(u => u.Role)
            .OrderBy(u => u.Login)
            .AsNoTracking()
            .ToListAsync();
    }
}