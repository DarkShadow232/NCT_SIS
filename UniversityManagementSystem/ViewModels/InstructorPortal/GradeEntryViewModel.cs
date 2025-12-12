using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.InstructorPortal;

public class GradeEntryViewModel : BaseViewModel
{
    private ObservableCollection<Course> _courses = new();
    private Course? _selectedCourse;
    private ObservableCollection<Grade> _grades = new();
    private Grade? _selectedGrade;
    private string _assignment1 = "";
    private string _assignment2 = "";
    private string _finalExam = "";
    private string _statusMessage = "";
    private bool _isSuccess;
    
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }
    
    public Course? SelectedCourse
    {
        get => _selectedCourse;
        set { SetProperty(ref _selectedCourse, value); LoadGrades(); }
    }
    
    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }
    
    public Grade? SelectedGrade
    {
        get => _selectedGrade;
        set { SetProperty(ref _selectedGrade, value); LoadGradeDetails(); }
    }
    
    public string Assignment1
    {
        get => _assignment1;
        set => SetProperty(ref _assignment1, value);
    }
    
    public string Assignment2
    {
        get => _assignment2;
        set => SetProperty(ref _assignment2, value);
    }
    
    public string FinalExam
    {
        get => _finalExam;
        set => SetProperty(ref _finalExam, value);
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    
    public bool IsSuccess
    {
        get => _isSuccess;
        set => SetProperty(ref _isSuccess, value);
    }
    
    public ICommand SaveGradeCommand { get; }
    public ICommand ClearFormCommand { get; }
    
    public GradeEntryViewModel()
    {
        SaveGradeCommand = new RelayCommand(_ => SaveGrade(), _ => SelectedGrade != null);
        ClearFormCommand = new RelayCommand(_ => ClearForm());
        LoadCourses();
    }
    
    private void LoadCourses()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.InstructorId == null) return;
        
        using var context = new UniversityDbContext();
        
        var courseIds = context.CourseInstructors
            .Where(ci => ci.InstructorId == user.InstructorId)
            .Select(ci => ci.CourseId)
            .ToList();
        
        var courses = context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .OrderBy(c => c.YearLevel)
            .ThenBy(c => c.Name)
            .ToList();
        
        Courses = new ObservableCollection<Course>(courses);
        
        if (Courses.Any())
        {
            SelectedCourse = Courses.First();
        }
    }
    
    private void LoadGrades()
    {
        if (SelectedCourse == null) return;
        
        using var context = new UniversityDbContext();
        
        var grades = context.Grades
            .Include(g => g.Student)
            .Where(g => g.CourseId == SelectedCourse.Id)
            .OrderBy(g => g.Student!.Name)
            .ToList();
        
        Grades = new ObservableCollection<Grade>(grades);
        SelectedGrade = null;
        ClearForm();
    }
    
    private void LoadGradeDetails()
    {
        if (SelectedGrade == null)
        {
            ClearForm();
            return;
        }
        
        Assignment1 = SelectedGrade.Assignment1?.ToString("F1") ?? "";
        Assignment2 = SelectedGrade.Assignment2?.ToString("F1") ?? "";
        FinalExam = SelectedGrade.FinalExam?.ToString("F1") ?? "";
        StatusMessage = "";
    }
    
    private void SaveGrade()
    {
        if (SelectedGrade == null) return;
        
        if (!double.TryParse(Assignment1, out var a1) || a1 < 0 || a1 > 20)
        {
            StatusMessage = "Assignment 1 must be between 0 and 20";
            IsSuccess = false;
            return;
        }
        
        if (!double.TryParse(Assignment2, out var a2) || a2 < 0 || a2 > 20)
        {
            StatusMessage = "Assignment 2 must be between 0 and 20";
            IsSuccess = false;
            return;
        }
        
        if (!double.TryParse(FinalExam, out var fe) || fe < 0 || fe > 60)
        {
            StatusMessage = "Final Exam must be between 0 and 60";
            IsSuccess = false;
            return;
        }
        
        try
        {
            using var context = new UniversityDbContext();
            
            var grade = context.Grades.Find(SelectedGrade.Id);
            if (grade == null) return;
            
            grade.Assignment1 = a1;
            grade.Assignment2 = a2;
            grade.FinalExam = fe;
            
            // Calculate total and symbolic grade
            var total = a1 + a2 + fe;
            grade.TotalScore = total;
            
            // Apply leniency if needed
            var gradingService = new GradingService();
            grade.SymbolicGrade = gradingService.ApplyLeniency(total);
            
            context.SaveChanges();
            
            StatusMessage = "Grade saved successfully!";
            IsSuccess = true;
            
            // Refresh the list
            LoadGrades();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving grade: {ex.Message}";
            IsSuccess = false;
        }
    }
    
    private void ClearForm()
    {
        Assignment1 = "";
        Assignment2 = "";
        FinalExam = "";
        StatusMessage = "";
    }
}

