using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.StudentPortal;

public class StudentDashboardViewModel : BaseViewModel
{
    private Student? _student;
    private int _totalCourses;
    private int _completedGrades;
    private decimal _gpa;
    private decimal _totalFees;
    private decimal _paidFees;
    private decimal _balanceDue;
    private List<Grade> _recentGrades = new();
    private List<Announcement> _announcements = new();
    
    public Student? Student
    {
        get => _student;
        set => SetProperty(ref _student, value);
    }
    
    public int TotalCourses
    {
        get => _totalCourses;
        set => SetProperty(ref _totalCourses, value);
    }
    
    public int CompletedGrades
    {
        get => _completedGrades;
        set => SetProperty(ref _completedGrades, value);
    }
    
    public decimal GPA
    {
        get => _gpa;
        set => SetProperty(ref _gpa, value);
    }
    
    public decimal TotalFees
    {
        get => _totalFees;
        set => SetProperty(ref _totalFees, value);
    }
    
    public decimal PaidFees
    {
        get => _paidFees;
        set => SetProperty(ref _paidFees, value);
    }
    
    public decimal BalanceDue
    {
        get => _balanceDue;
        set => SetProperty(ref _balanceDue, value);
    }
    
    public string CurrentDate => DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    
    public List<Grade> RecentGrades
    {
        get => _recentGrades;
        set => SetProperty(ref _recentGrades, value);
    }
    
    public List<Announcement> Announcements
    {
        get => _announcements;
        set => SetProperty(ref _announcements, value);
    }
    
    public string WelcomeMessage => $"Welcome, {Student?.Name ?? "Student"}!";
    public string DepartmentInfo => Student?.Department?.Name ?? "Not Assigned";
    public string YearInfo => $"Year {Student?.YearLevel ?? 1}";
    public string SectionInfo => Student?.Section?.Name ?? "Not Assigned";
    
    public StudentDashboardViewModel()
    {
        LoadData();
    }
    
    private void LoadData()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.StudentId == null) return;
        
        using var context = new UniversityDbContext();
        
        Student = context.Students
            .Include(s => s.Department)
            .Include(s => s.Section)
            .FirstOrDefault(s => s.Id == user.StudentId);
        
        if (Student == null) return;
        
        // Load grades
        var grades = context.Grades
            .Include(g => g.Course)
            .Where(g => g.StudentId == Student.Id)
            .ToList();
        
        TotalCourses = grades.Select(g => g.CourseId).Distinct().Count();
        CompletedGrades = grades.Count(g => g.SymbolicGrade != "NA");
        
        // Calculate GPA
        var completedGrades = grades.Where(g => g.SymbolicGrade != "NA" && g.TotalScore.HasValue).ToList();
        if (completedGrades.Any())
        {
            var avgScore = completedGrades.Average(g => g.TotalScore ?? 0);
            GPA = (decimal)avgScore / 25; // Convert to 4.0 scale
        }
        
        RecentGrades = grades.OrderByDescending(g => g.Id).Take(5).ToList();
        
        // Load fees
        var fees = context.StudentFees.Where(f => f.StudentId == Student.Id).ToList();
        TotalFees = fees.Sum(f => f.Amount);
        PaidFees = fees.Sum(f => f.AmountPaid);
        BalanceDue = TotalFees - PaidFees;
        
        // Load announcements
        Announcements = context.Announcements
            .Include(a => a.CreatedByUser)
            .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.PublishDate)
            .Take(5)
            .ToList();
        
        OnPropertyChanged(nameof(WelcomeMessage));
        OnPropertyChanged(nameof(DepartmentInfo));
        OnPropertyChanged(nameof(YearInfo));
        OnPropertyChanged(nameof(SectionInfo));
    }
}

