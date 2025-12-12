using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Services;

public class AuthenticationService
{
    private static AuthenticationService? _instance;
    public static AuthenticationService Instance => _instance ??= new AuthenticationService();
    
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    public bool IsStudent => CurrentUser?.Role == "Student";
    public bool IsInstructor => CurrentUser?.Role == "Instructor";
    
    public event EventHandler? UserLoggedIn;
    public event EventHandler? UserLoggedOut;
    
    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        try
        {
            using var context = new UniversityDbContext();
            var user = await context.Users
                .Include(u => u.Student)
                .Include(u => u.Instructor)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            
            if (user == null)
            {
                return (false, "Invalid username or password");
            }
            
            if (!VerifyPassword(password, user.PasswordHash))
            {
                return (false, "Invalid username or password");
            }
            
            user.LastLogin = DateTime.Now;
            await context.SaveChangesAsync();
            
            CurrentUser = user;
            UserLoggedIn?.Invoke(this, EventArgs.Empty);
            
            return (true, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, $"Login error: {ex.Message}");
        }
    }
    
    public void Logout()
    {
        CurrentUser = null;
        UserLoggedOut?.Invoke(this, EventArgs.Empty);
    }
    
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
    
    public static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}

