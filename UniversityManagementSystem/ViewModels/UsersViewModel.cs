using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class UsersViewModel : BaseViewModel
{
    private ObservableCollection<User> _users = new();
    private User? _selectedUser;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _selectedRole = "Student";
    private bool _isActive = true;
    private Student? _linkedStudent;
    private Instructor? _linkedInstructor;
    private bool _isEditing;
    private string _errorMessage = string.Empty;
    
    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }
    
    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            SetProperty(ref _selectedUser, value);
            if (value != null)
            {
                Username = value.Username;
                SelectedRole = value.Role;
                IsActive = value.IsActive;
                LinkedStudent = value.Student;
                LinkedInstructor = value.Instructor;
                IsEditing = true;
            }
        }
    }
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
    
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }
    
    public string SelectedRole
    {
        get => _selectedRole;
        set => SetProperty(ref _selectedRole, value);
    }
    
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
    
    public Student? LinkedStudent
    {
        get => _linkedStudent;
        set => SetProperty(ref _linkedStudent, value);
    }
    
    public Instructor? LinkedInstructor
    {
        get => _linkedInstructor;
        set => SetProperty(ref _linkedInstructor, value);
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    
    public List<string> Roles { get; } = new() { "Admin", "Student", "Instructor" };
    
    public ObservableCollection<Student> AvailableStudents { get; set; } = new();
    public ObservableCollection<Instructor> AvailableInstructors { get; set; } = new();
    
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ResetPasswordCommand { get; }
    
    public UsersViewModel()
    {
        AddCommand = new RelayCommand(AddUser);
        UpdateCommand = new RelayCommand(UpdateUser);
        DeleteCommand = new RelayCommand(DeleteUser);
        ClearCommand = new RelayCommand(ClearForm);
        ResetPasswordCommand = new RelayCommand(ResetPassword);
        
        LoadData();
    }
    
    private void LoadData()
    {
        using var context = new UniversityDbContext();
        
        var users = context.Users
            .Include(u => u.Student)
            .Include(u => u.Instructor)
            .OrderBy(u => u.Username)
            .ToList();
        Users = new ObservableCollection<User>(users);
        
        // Load available students not already linked
        var linkedStudentIds = users.Where(u => u.StudentId != null).Select(u => u.StudentId).ToList();
        var availableStudents = context.Students
            .Where(s => !linkedStudentIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToList();
        AvailableStudents = new ObservableCollection<Student>(availableStudents);
        
        // Load available instructors not already linked
        var linkedInstructorIds = users.Where(u => u.InstructorId != null).Select(u => u.InstructorId).ToList();
        var availableInstructors = context.Instructors
            .Where(i => !linkedInstructorIds.Contains(i.Id))
            .OrderBy(i => i.Name)
            .ToList();
        AvailableInstructors = new ObservableCollection<Instructor>(availableInstructors);
    }
    
    private void AddUser()
    {
        ErrorMessage = string.Empty;
        
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Username is required";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Password is required";
            return;
        }
        
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match";
            return;
        }
        
        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters";
            return;
        }
        
        using var context = new UniversityDbContext();
        
        if (context.Users.Any(u => u.Username.ToLower() == Username.ToLower()))
        {
            ErrorMessage = "Username already exists";
            return;
        }
        
        var user = new User
        {
            Username = Username,
            PasswordHash = AuthenticationService.HashPassword(Password),
            Role = SelectedRole,
            IsActive = IsActive,
            StudentId = SelectedRole == "Student" ? LinkedStudent?.Id : null,
            InstructorId = SelectedRole == "Instructor" ? LinkedInstructor?.Id : null
        };
        
        context.Users.Add(user);
        context.SaveChanges();
        
        MessageBox.Show($"User '{Username}' created successfully!\nPassword: {Password}", "User Created", 
            MessageBoxButton.OK, MessageBoxImage.Information);
        
        LoadData();
        ClearForm();
    }
    
    private void UpdateUser()
    {
        ErrorMessage = string.Empty;
        
        if (SelectedUser == null)
        {
            ErrorMessage = "Please select a user to update";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Username is required";
            return;
        }
        
        using var context = new UniversityDbContext();
        
        int selectedUserId = SelectedUser.Id;
        var user = context.Users.Find(selectedUserId);
        if (user == null)
        {
            ErrorMessage = "User not found";
            return;
        }
        
        if (context.Users.Any(u => u.Username.ToLower() == Username.ToLower() && u.Id != selectedUserId))
        {
            ErrorMessage = "Username already exists";
            return;
        }
        
        user.Username = Username;
        user.Role = SelectedRole;
        user.IsActive = IsActive;
        user.StudentId = SelectedRole == "Student" ? LinkedStudent?.Id : null;
        user.InstructorId = SelectedRole == "Instructor" ? LinkedInstructor?.Id : null;
        
        context.SaveChanges();
        
        MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        
        LoadData();
        ClearForm();
    }
    
    private void DeleteUser()
    {
        if (SelectedUser == null)
        {
            MessageBox.Show("Please select a user to delete", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var result = MessageBox.Show($"Are you sure you want to delete user '{SelectedUser.Username}'?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            int selectedUserId = SelectedUser.Id;
            var user = context.Users.Find(selectedUserId);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
            
            LoadData();
            ClearForm();
        }
    }
    
    private void ResetPassword()
    {
        ErrorMessage = string.Empty;
        
        if (SelectedUser == null)
        {
            ErrorMessage = "Please select a user";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Enter new password";
            return;
        }
        
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match";
            return;
        }
        
        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters";
            return;
        }
        
        using var context = new UniversityDbContext();
        int selectedUserId = SelectedUser.Id;
        var user = context.Users.Find(selectedUserId);
        if (user != null)
        {
            user.PasswordHash = AuthenticationService.HashPassword(Password);
            context.SaveChanges();
            
            MessageBox.Show($"Password reset successfully!\nNew Password: {Password}", "Password Reset",
                MessageBoxButton.OK, MessageBoxImage.Information);
            
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
    
    private void ClearForm()
    {
        SelectedUser = null;
        Username = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        SelectedRole = "Student";
        IsActive = true;
        LinkedStudent = null;
        LinkedInstructor = null;
        IsEditing = false;
        ErrorMessage = string.Empty;
    }
}

