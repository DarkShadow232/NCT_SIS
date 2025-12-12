using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.InstructorPortal;

public class InstructorDashboardViewModel : BaseViewModel
{
    private Instructor? _instructor;
    private int _totalCourses;
    private int _totalStudents;
    private int _pendingGrades;
    private int _completedGrades;
    private ObservableCollection<CourseInstructor> _myCourses = new();
    private ObservableCollection<Announcement> _announcements = new();
    
    public Instructor? Instructor
    {
        get => _instructor;
        set => SetProperty(ref _instructor, value);
    }
    
    public int TotalCourses
    {
        get => _totalCourses;
        set => SetProperty(ref _totalCourses, value);
    }
    
    public int TotalStudents
    {
        get => _totalStudents;
        set => SetProperty(ref _totalStudents, value);
    }
    
    public int PendingGrades
    {
        get => _pendingGrades;
        set => SetProperty(ref _pendingGrades, value);
    }
    
    public int CompletedGrades
    {
        get => _completedGrades;
        set => SetProperty(ref _completedGrades, value);
    }
    
    public ObservableCollection<CourseInstructor> MyCourses
    {
        get => _myCourses;
        set => SetProperty(ref _myCourses, value);
    }
    
    public ObservableCollection<Announcement> Announcements
    {
        get => _announcements;
        set => SetProperty(ref _announcements, value);
    }
    
    public string CurrentDate => DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    
    public string WelcomeMessage => $"Welcome, {Instructor?.Name ?? "Instructor"}!";
    public string DepartmentInfo => Instructor?.Department?.Name ?? "Department";
    
    public InstructorDashboardViewModel()
    {
        LoadData();
    }
    
    private void LoadData()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.InstructorId == null) return;
        
        using var context = new UniversityDbContext();
        
        Instructor = context.Instructors
            .Include(i => i.Department)
            .FirstOrDefault(i => i.Id == user.InstructorId);
        
        if (Instructor == null) return;
        
        var courseInstructors = context.CourseInstructors
            .Include(ci => ci.Course)
            .Where(ci => ci.InstructorId == Instructor.Id)
            .ToList();
        
        MyCourses = new ObservableCollection<CourseInstructor>(courseInstructors);
        TotalCourses = courseInstructors.Count;
        
        // Count students and grades
        var courseIds = courseInstructors.Select(ci => ci.CourseId).ToList();
        var grades = context.Grades
            .Where(g => courseIds.Contains(g.CourseId))
            .ToList();
        
        TotalStudents = grades.Select(g => g.StudentId).Distinct().Count();
        CompletedGrades = grades.Count(g => g.SymbolicGrade != "NA");
        PendingGrades = grades.Count(g => g.SymbolicGrade == "NA");
        
        // Load announcements
        Announcements = new ObservableCollection<Announcement>(
            context.Announcements
                .Include(a => a.CreatedByUser)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.PublishDate)
                .Take(5)
                .ToList());
        
        OnPropertyChanged(nameof(WelcomeMessage));
        OnPropertyChanged(nameof(DepartmentInfo));
    }
}

