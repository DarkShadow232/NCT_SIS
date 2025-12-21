using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class GradesViewModel : BaseViewModel
{
    private readonly GradingService _gradingService = new();
    
    private ObservableCollection<Grade> _grades = new();
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Course> _courses = new();
    private Grade? _selectedGrade;
    private int? _filterStudentId;
    private int? _filterCourseId;
    private bool _isEditing;
    private bool _isAdding;
    
    // Form fields
    private int _formStudentId;
    private int _formCourseId;
    private string _formAssignment1 = string.Empty;
    private string _formAssignment2 = string.Empty;
    private string _formCourseWork = string.Empty;  // New field for CW
    private string _formFinalExam = string.Empty;
    private double? _formTotalScore;
    private string? _formSymbolicGrade;
    private bool _formLeniencyApplied;
    
    // Validation errors
    private string _assignment1Error = string.Empty;
    private string _assignment2Error = string.Empty;
    private string _courseWorkError = string.Empty;  // New validation error
    private string _finalExamError = string.Empty;
    
    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }
    
    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }
    
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }
    
    public Grade? SelectedGrade
    {
        get => _selectedGrade;
        set
        {
            if (SetProperty(ref _selectedGrade, value) && value != null && !_isAdding)
            {
                LoadGradeToForm(value);
                ClearValidationErrors();
            }
        }
    }
    
    public int? FilterStudentId
    {
        get => _filterStudentId;
        set
        {
            if (SetProperty(ref _filterStudentId, value))
                LoadGrades();
        }
    }
    
    public int? FilterCourseId
    {
        get => _filterCourseId;
        set
        {
            if (SetProperty(ref _filterCourseId, value))
                LoadGrades();
        }
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public bool IsAdding
    {
        get => _isAdding;
        set => SetProperty(ref _isAdding, value);
    }
    
    // Form properties with validation
    public int FormStudentId
    {
        get => _formStudentId;
        set => SetProperty(ref _formStudentId, value);
    }
    
    public int FormCourseId
    {
        get => _formCourseId;
        set => SetProperty(ref _formCourseId, value);
    }
    
    public string FormAssignment1
    {
        get => _formAssignment1;
        set
        {
            if (SetProperty(ref _formAssignment1, value))
            {
                ValidateAssignment1();
                CalculatePreview();
            }
        }
    }
    
    public string FormAssignment2
    {
        get => _formAssignment2;
        set
        {
            if (SetProperty(ref _formAssignment2, value))
            {
                ValidateAssignment2();
                CalculatePreview();
            }
        }
    }
    
    public string FormCourseWork
    {
        get => _formCourseWork;
        set
        {
            if (SetProperty(ref _formCourseWork, value))
            {
                ValidateCourseWork();
                CalculatePreview();
            }
        }
    }
    
    public string FormFinalExam
    {
        get => _formFinalExam;
        set
        {
            if (SetProperty(ref _formFinalExam, value))
            {
                ValidateFinalExam();
                CalculatePreview();
            }
        }
    }
    
    public double? FormTotalScore
    {
        get => _formTotalScore;
        set => SetProperty(ref _formTotalScore, value);
    }
    
    public string? FormSymbolicGrade
    {
        get => _formSymbolicGrade;
        set => SetProperty(ref _formSymbolicGrade, value);
    }
    
    public bool FormLeniencyApplied
    {
        get => _formLeniencyApplied;
        set => SetProperty(ref _formLeniencyApplied, value);
    }
    
    // Validation errors
    public string Assignment1Error
    {
        get => _assignment1Error;
        set => SetProperty(ref _assignment1Error, value);
    }
    
    public string Assignment2Error
    {
        get => _assignment2Error;
        set => SetProperty(ref _assignment2Error, value);
    }
    
    public string CourseWorkError
    {
        get => _courseWorkError;
        set => SetProperty(ref _courseWorkError, value);
    }
    
    public string FinalExamError
    {
        get => _finalExamError;
        set => SetProperty(ref _finalExamError, value);
    }
    
    public bool HasErrors => !string.IsNullOrEmpty(Assignment1Error) || 
                             !string.IsNullOrEmpty(Assignment2Error) || 
                             !string.IsNullOrEmpty(CourseWorkError) ||
                             !string.IsNullOrEmpty(FinalExamError);
    
    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand RecalculateAllCommand { get; }
    
    public GradesViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedGrade != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedGrade != null);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        RecalculateAllCommand = new RelayCommand(RecalculateAll);
        
        LoadStudents();
        LoadCourses();
        LoadGrades();
    }
    
    private void ValidateAssignment1()
    {
        if (string.IsNullOrWhiteSpace(FormAssignment1))
        {
            Assignment1Error = string.Empty;
            return;
        }
        
        if (!double.TryParse(FormAssignment1, out var score))
        {
            Assignment1Error = "Please enter a valid number.";
        }
        else if (score < 0 || score > 100)
        {
            Assignment1Error = "Score must be between 0 and 100.";
        }
        else
        {
            Assignment1Error = string.Empty;
        }
    }
    
    private void ValidateCourseWork()
    {
        if (string.IsNullOrWhiteSpace(FormCourseWork))
        {
            CourseWorkError = string.Empty;
            return;
        }
        
        if (!double.TryParse(FormCourseWork, out var score))
        {
            CourseWorkError = "Please enter a valid number.";
        }
        else if (score < 0 || score > 100)
        {
            CourseWorkError = "Score must be between 0 and 100.";
        }
        else
        {
            CourseWorkError = string.Empty;
        }
    }
    
    private void ValidateAssignment2()
    {
        if (string.IsNullOrWhiteSpace(FormAssignment2))
        {
            Assignment2Error = string.Empty;
            return;
        }
        
        if (!double.TryParse(FormAssignment2, out var score))
        {
            Assignment2Error = "Please enter a valid number.";
        }
        else if (score < 0 || score > 100)
        {
            Assignment2Error = "Score must be between 0 and 100.";
        }
        else
        {
            Assignment2Error = string.Empty;
        }
    }
    
    private void ValidateFinalExam()
    {
        // Get course to check if Final Exam is required
        var course = Courses.FirstOrDefault(c => c.Id == FormCourseId);
        
        // For Theoretical courses, Final Exam is not required (should be empty)
        if (course?.CourseType == CourseType.Theoretical100)
        {
            FinalExamError = string.Empty;
            return;
        }
        
        // For Practical courses, Final Exam is required
        if (string.IsNullOrWhiteSpace(FormFinalExam))
        {
            FinalExamError = "Final Exam is required for practical courses.";
            return;
        }
        
        if (!double.TryParse(FormFinalExam, out var score))
        {
            FinalExamError = "Please enter a valid number.";
        }
        else if (score < 0 || score > 100)
        {
            FinalExamError = "Score must be between 0 and 100.";
        }
        else
        {
            FinalExamError = string.Empty;
        }
    }
    
    private void ClearValidationErrors()
    {
        Assignment1Error = string.Empty;
        Assignment2Error = string.Empty;
        CourseWorkError = string.Empty;
        FinalExamError = string.Empty;
    }
    
    private bool ValidateAll()
    {
        ValidateAssignment1();
        ValidateAssignment2();
        ValidateFinalExam();
        return !HasErrors;
    }
    
    private void LoadGrades()
    {
        using var context = new UniversityDbContext();
        var query = context.Grades
            .Include(g => g.Student)
            .Include(g => g.Course)
            .AsQueryable();
        
        if (FilterStudentId.HasValue)
        {
            query = query.Where(g => g.StudentId == FilterStudentId.Value);
        }
        
        if (FilterCourseId.HasValue)
        {
            query = query.Where(g => g.CourseId == FilterCourseId.Value);
        }
        
        Grades = new ObservableCollection<Grade>(
            query.OrderBy(g => g.Student!.Name).ThenBy(g => g.Course!.Code).ToList()
        );
    }
    
    private void LoadStudents()
    {
        using var context = new UniversityDbContext();
        Students = new ObservableCollection<Student>(context.Students.OrderBy(s => s.Name).ToList());
        
        if (Students.Any())
            FormStudentId = Students.First().Id;
    }
    
    private void LoadCourses()
    {
        using var context = new UniversityDbContext();
        Courses = new ObservableCollection<Course>(context.Courses.OrderBy(c => c.Code).ToList());
        
        if (Courses.Any())
            FormCourseId = Courses.First().Id;
    }
    
    private void LoadGradeToForm(Grade grade)
    {
        FormStudentId = grade.StudentId;
        FormCourseId = grade.CourseId;
        FormAssignment1 = grade.Assignment1?.ToString() ?? string.Empty;
        FormAssignment2 = grade.Assignment2?.ToString() ?? string.Empty;
        FormCourseWork = grade.CourseWork?.ToString() ?? string.Empty;
        FormFinalExam = grade.FinalExam?.ToString() ?? string.Empty;
        FormTotalScore = grade.TotalScore;
        FormSymbolicGrade = grade.SymbolicGrade;
        FormLeniencyApplied = grade.LeniencyApplied;
    }
    
    private void ClearForm()
    {
        FormStudentId = Students.FirstOrDefault()?.Id ?? 0;
        FormCourseId = Courses.FirstOrDefault()?.Id ?? 0;
        FormAssignment1 = string.Empty;
        FormAssignment2 = string.Empty;
        FormCourseWork = string.Empty;
        FormFinalExam = string.Empty;
        FormTotalScore = null;
        FormSymbolicGrade = null;
        FormLeniencyApplied = false;
        ClearValidationErrors();
    }
    
    private void CalculatePreview()
    {
        double? a1 = double.TryParse(FormAssignment1, out var v1) ? v1 : null;
        double? a2 = double.TryParse(FormAssignment2, out var v2) ? v2 : null;
        double? cw = double.TryParse(FormCourseWork, out var v3) ? v3 : null;
        double? final = double.TryParse(FormFinalExam, out var v4) ? v4 : null;
        
        // Get course to check if all required fields are present
        var course = Courses.FirstOrDefault(c => c.Id == FormCourseId);
        if (course == null)
        {
            FormTotalScore = null;
            FormSymbolicGrade = null;
            FormLeniencyApplied = false;
            return;
        }
        
        bool isComplete = (course.CourseType == CourseType.Theoretical100 && a1.HasValue && a2.HasValue && cw.HasValue) ||
                         (course.CourseType != CourseType.Theoretical100 && a1.HasValue && a2.HasValue && cw.HasValue && final.HasValue);
        
        if (isComplete && !HasErrors)
        {
            var tempGrade = new Grade
            {
                Assignment1 = a1,
                Assignment2 = a2,
                CourseWork = cw,
                FinalExam = final,
                Course = course
            };
            _gradingService.CalculateGrade(tempGrade);
            FormTotalScore = tempGrade.TotalScore;
            FormSymbolicGrade = tempGrade.SymbolicGrade;
            FormLeniencyApplied = tempGrade.LeniencyApplied;
        }
        else
        {
            FormTotalScore = null;
            FormSymbolicGrade = null;
            FormLeniencyApplied = false;
        }
    }
    
    private void StartAdd()
    {
        ClearForm();
        IsAdding = true;
        IsEditing = true;
        SelectedGrade = null;
    }
    
    private void StartEdit()
    {
        if (SelectedGrade != null)
        {
            IsAdding = false;
            IsEditing = true;
            ClearValidationErrors();
        }
    }
    
    private void Save()
    {
        if (!ValidateAll())
        {
            MessageBox.Show("Please fix the validation errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        double? a1 = double.TryParse(FormAssignment1, out var v1) ? v1 : null;
        double? a2 = double.TryParse(FormAssignment2, out var v2) ? v2 : null;
        double? cw = double.TryParse(FormCourseWork, out var v3) ? v3 : null;
        double? final = double.TryParse(FormFinalExam, out var v4) ? v4 : null;
        
        using var context = new UniversityDbContext();
        
        if (IsAdding)
        {
            // Check if grade already exists for this student-course combination
            if (context.Grades.Any(g => g.StudentId == FormStudentId && g.CourseId == FormCourseId))
            {
                MessageBox.Show("A grade record already exists for this student in this course.", 
                    "Duplicate Record", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var grade = new Grade
            {
                StudentId = FormStudentId,
                CourseId = FormCourseId,
                Assignment1 = a1,
                Assignment2 = a2,
                CourseWork = cw,
                FinalExam = final
            };
            
            // Load course for calculation
            grade.Course = context.Courses.Find(FormCourseId);
            _gradingService.CalculateGrade(grade);
            context.Grades.Add(grade);
            context.SaveChanges();
            
            MessageBox.Show("Grade recorded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (SelectedGrade != null)
        {
            var grade = context.Grades.Include(g => g.Course).FirstOrDefault(g => g.Id == SelectedGrade.Id);
            if (grade != null)
            {
                grade.Assignment1 = a1;
                grade.Assignment2 = a2;
                grade.CourseWork = cw;
                grade.FinalExam = final;
                _gradingService.CalculateGrade(grade);
                context.SaveChanges();
                
                MessageBox.Show("Grade updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        IsEditing = false;
        IsAdding = false;
        LoadGrades();
    }
    
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        ClearValidationErrors();
        if (SelectedGrade != null)
            LoadGradeToForm(SelectedGrade);
        else
            ClearForm();
    }
    
    private void Delete()
    {
        if (SelectedGrade == null) return;
        
        var studentName = SelectedGrade.Student?.Name ?? "Unknown";
        var courseName = SelectedGrade.Course?.Name ?? "Unknown";
        
        var result = MessageBox.Show($"Are you sure you want to delete the grade for {studentName} in {courseName}?", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var grade = context.Grades.Find(SelectedGrade.Id);
            if (grade != null)
            {
                context.Grades.Remove(grade);
                context.SaveChanges();
                MessageBox.Show("Grade deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadGrades();
            ClearForm();
        }
    }
    
    private void ClearFilter()
    {
        FilterStudentId = null;
        FilterCourseId = null;
    }
    
    private void RecalculateAll()
    {
        var result = MessageBox.Show("This will recalculate all grades with the leniency algorithm.\n\nContinue?", 
            "Recalculate Grades", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                
                using var context = new UniversityDbContext();
                // IMPORTANT: Include Course and Student to ensure CalculateGrade works properly
                var allGrades = context.Grades
                    .Include(g => g.Course)
                    .Include(g => g.Student)
                    .ToList();
                
                int recalculatedCount = 0;
                foreach (var grade in allGrades)
                {
                    // Only recalculate if grade has required data
                    if (grade.Course != null)
                    {
                        _gradingService.CalculateGrade(grade);
                        recalculatedCount++;
                    }
                }
                
                context.SaveChanges();
                LoadGrades(); // Reload to refresh the UI
                
                SuccessMessage = $"Successfully recalculated {recalculatedCount} grades.";
                MessageBox.Show($"Successfully recalculated {recalculatedCount} grades.", 
                    "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to recalculate grades: {ex.Message}";
                MessageBox.Show($"Failed to recalculate grades: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
