using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.InstructorPortal;

public class CourseStats
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public int YearLevel { get; set; }
    public string Semester { get; set; } = "";
    public int TotalStudents { get; set; }
    public int GradedStudents { get; set; }
    public int PendingGrades { get; set; }
    public double AverageScore { get; set; }
    public Course? Course { get; set; }
}

public class MyCoursesViewModel : BaseViewModel
{
    private ObservableCollection<CourseStats> _courses = new();
    private CourseStats? _selectedCourse;
    private ObservableCollection<Grade> _courseGrades = new();
    
    public ObservableCollection<CourseStats> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }
    
    public CourseStats? SelectedCourse
    {
        get => _selectedCourse;
        set { SetProperty(ref _selectedCourse, value); LoadCourseGrades(); }
    }
    
    public ObservableCollection<Grade> CourseGrades
    {
        get => _courseGrades;
        set => SetProperty(ref _courseGrades, value);
    }
    
    public ICommand SelectCourseCommand { get; }
    
    public MyCoursesViewModel()
    {
        SelectCourseCommand = new RelayCommand(c => SelectedCourse = c as CourseStats);
        LoadCourses();
    }
    
    private void LoadCourses()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.InstructorId == null) return;
        
        using var context = new UniversityDbContext();
        
        var courseInstructors = context.CourseInstructors
            .Include(ci => ci.Course)
            .Where(ci => ci.InstructorId == user.InstructorId)
            .ToList();
        
        var courseStats = new List<CourseStats>();
        
        foreach (var ci in courseInstructors)
        {
            var grades = context.Grades
                .Where(g => g.CourseId == ci.CourseId)
                .ToList();
            
            var graded = grades.Where(g => g.SymbolicGrade != "NA").ToList();
            
            courseStats.Add(new CourseStats
            {
                CourseId = ci.CourseId,
                CourseCode = ci.Course?.Code ?? "",
                CourseName = ci.Course?.Name ?? "",
                YearLevel = ci.Course?.YearLevel ?? 1,
                Semester = ci.Semester,
                TotalStudents = grades.Count,
                GradedStudents = graded.Count,
                PendingGrades = grades.Count - graded.Count,
                AverageScore = graded.Any() ? graded.Average(g => g.TotalScore) ?? 0 : 0,
                Course = ci.Course
            });
        }
        
        Courses = new ObservableCollection<CourseStats>(courseStats);
        
        if (Courses.Any())
        {
            SelectedCourse = Courses.First();
        }
    }
    
    private void LoadCourseGrades()
    {
        if (SelectedCourse == null) return;
        
        using var context = new UniversityDbContext();
        
        var grades = context.Grades
            .Include(g => g.Student)
            .Include(g => g.Course)
            .Where(g => g.CourseId == SelectedCourse.CourseId)
            .OrderBy(g => g.Student!.Name)
            .ToList();
        
        CourseGrades = new ObservableCollection<Grade>(grades);
    }
}

