using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class AttendanceViewModel : BaseViewModel
{
    private readonly FileExportService _fileExportService = new();
    
    private ObservableCollection<Attendance> _attendances = new();
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Course> _courses = new();
    private ObservableCollection<Section> _sections = new();
    private Attendance? _selectedAttendance;
    private Student? _selectedStudent;
    private Course? _selectedCourse;
    private Section? _selectedSection;
    private DateTime _selectedDate = DateTime.Today;
    private bool _isPresent = true;
    private bool _isLecture = true;
    private string _notes = string.Empty;
    private string _searchText = string.Empty;
    private int? _filterCourseId;
    
    public ObservableCollection<Attendance> Attendances
    {
        get => _attendances;
        set => SetProperty(ref _attendances, value);
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
    
    public ObservableCollection<Section> Sections
    {
        get => _sections;
        set => SetProperty(ref _sections, value);
    }
    
    public Attendance? SelectedAttendance
    {
        get => _selectedAttendance;
        set => SetProperty(ref _selectedAttendance, value);
    }
    
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set => SetProperty(ref _selectedStudent, value);
    }
    
    public Course? SelectedCourse
    {
        get => _selectedCourse;
        set
        {
            if (SetProperty(ref _selectedCourse, value))
                LoadSectionsForCourse();
        }
    }
    
    public Section? SelectedSection
    {
        get => _selectedSection;
        set => SetProperty(ref _selectedSection, value);
    }
    
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }
    
    public bool IsPresent
    {
        get => _isPresent;
        set => SetProperty(ref _isPresent, value);
    }
    
    public bool IsLecture
    {
        get => _isLecture;
        set => SetProperty(ref _isLecture, value);
    }
    
    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                LoadAttendances();
        }
    }
    
    public int? FilterCourseId
    {
        get => _filterCourseId;
        set
        {
            if (SetProperty(ref _filterCourseId, value))
                LoadAttendances();
        }
    }
    
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RefreshCommand { get; }
    
    public AttendanceViewModel()
    {
        SaveCommand = new RelayCommand(SaveAttendance);
        DeleteCommand = new RelayCommand(DeleteAttendance, () => SelectedAttendance != null);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        ExportCommand = new RelayCommand(ExportAttendance);
        RefreshCommand = new RelayCommand(LoadAttendances);
        
        LoadData();
    }
    
    private void LoadData()
    {
        LoadStudents();
        LoadCourses();
        LoadAttendances();
    }
    
    private void LoadStudents()
    {
        using var context = new UniversityDbContext();
        var students = context.Students
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToList();
        Students = new ObservableCollection<Student>(students);
    }
    
    private void LoadCourses()
    {
        using var context = new UniversityDbContext();
        var courses = context.Courses
            .Include(c => c.Department)
            .OrderBy(c => c.Name)
            .ToList();
        Courses = new ObservableCollection<Course>(courses);
    }
    
    private void LoadSectionsForCourse()
    {
        if (SelectedCourse == null)
        {
            Sections = new ObservableCollection<Section>();
            return;
        }
        
        using var context = new UniversityDbContext();
        var sections = context.Sections
            .Where(s => s.CourseId == SelectedCourse.Id)
            .OrderBy(s => s.Name)
            .ToList();
        Sections = new ObservableCollection<Section>(sections);
    }
    
    private void LoadAttendances()
    {
        using var context = new UniversityDbContext();
        var query = context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Course)
            .Include(a => a.Section)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(a => a.Student != null && a.Student.Name.Contains(SearchText));
        }
        
        if (FilterCourseId.HasValue)
        {
            query = query.Where(a => a.CourseId == FilterCourseId.Value);
        }
        
        var attendances = query.OrderByDescending(a => a.Date).ToList();
        Attendances = new ObservableCollection<Attendance>(attendances);
    }
    
    private void SaveAttendance()
    {
        if (SelectedStudent == null || SelectedCourse == null)
        {
            MessageBox.Show("Please select a student and course.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        var attendance = new Attendance
        {
            StudentId = SelectedStudent.Id,
            CourseId = SelectedCourse.Id,
            SectionId = SelectedSection?.Id,
            Date = SelectedDate,
            IsPresent = IsPresent,
            IsLecture = IsLecture,
            Notes = Notes
        };
        
        context.Attendances.Add(attendance);
        context.SaveChanges();
        
        MessageBox.Show("Attendance recorded successfully!", "Success", 
            MessageBoxButton.OK, MessageBoxImage.Information);
        
        LoadAttendances();
        ClearForm();
    }
    
    private void DeleteAttendance()
    {
        if (SelectedAttendance == null) return;
        
        var result = MessageBox.Show(
            "Are you sure you want to delete this attendance record?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var attendance = context.Attendances.Find(SelectedAttendance.Id);
        if (attendance != null)
        {
            context.Attendances.Remove(attendance);
            context.SaveChanges();
            LoadAttendances();
            MessageBox.Show("Attendance deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void ClearFilter()
    {
        SearchText = string.Empty;
        FilterCourseId = null;
    }
    
    private void ExportAttendance()
    {
        if (!Attendances.Any())
        {
            MessageBox.Show("No attendance records to export.", "Information", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        
        try
        {
            var fileName = _fileExportService.ExportAttendance(Attendances, "Attendance Report");
            MessageBox.Show($"Attendance exported successfully!\n\nFile: {fileName}", 
                "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting attendance: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void ClearForm()
    {
        SelectedStudent = null;
        SelectedCourse = null;
        SelectedSection = null;
        SelectedDate = DateTime.Today;
        IsPresent = true;
        IsLecture = true;
        Notes = string.Empty;
    }
}

