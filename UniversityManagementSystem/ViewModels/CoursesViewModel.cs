using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.ViewModels;

public class CoursesViewModel : BaseViewModel
{
    private ObservableCollection<Course> _courses = new();
    private ObservableCollection<Department> _departments = new();
    private Course? _selectedCourse;
    private int? _filterYearLevel;
    private int? _filterDepartmentId;
    private bool _isEditing;
    private bool _isAdding;
    
    // Form fields
    private string _formName = string.Empty;
    private string _formCode = string.Empty;
    private int _formCredits = 3;
    private int _formYearLevel = 1;
    private int _formDepartmentId;
    private string _formDescription = string.Empty;
    
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }
    
    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }
    
    public Course? SelectedCourse
    {
        get => _selectedCourse;
        set
        {
            if (SetProperty(ref _selectedCourse, value) && value != null && !_isAdding)
            {
                LoadCourseToForm(value);
            }
        }
    }
    
    public int? FilterYearLevel
    {
        get => _filterYearLevel;
        set
        {
            if (SetProperty(ref _filterYearLevel, value))
                LoadCourses();
        }
    }
    
    public int? FilterDepartmentId
    {
        get => _filterDepartmentId;
        set
        {
            if (SetProperty(ref _filterDepartmentId, value))
                LoadCourses();
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
    
    // Form properties
    public string FormName
    {
        get => _formName;
        set => SetProperty(ref _formName, value);
    }
    
    public string FormCode
    {
        get => _formCode;
        set => SetProperty(ref _formCode, value);
    }
    
    public int FormCredits
    {
        get => _formCredits;
        set => SetProperty(ref _formCredits, value);
    }
    
    public int FormYearLevel
    {
        get => _formYearLevel;
        set => SetProperty(ref _formYearLevel, value);
    }
    
    public int FormDepartmentId
    {
        get => _formDepartmentId;
        set => SetProperty(ref _formDepartmentId, value);
    }
    
    public string FormDescription
    {
        get => _formDescription;
        set => SetProperty(ref _formDescription, value);
    }
    
    public int[] YearLevels => new[] { 1, 2, 3, 4 };
    public int[] CreditOptions => new[] { 1, 2, 3, 4, 5, 6 };
    
    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    
    public CoursesViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedCourse != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedCourse != null);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        
        LoadDepartments();
        LoadCourses();
    }
    
    private void LoadCourses()
    {
        using var context = new UniversityDbContext();
        var query = context.Courses.Include(c => c.Department).AsQueryable();
        
        if (FilterYearLevel.HasValue)
        {
            query = query.Where(c => c.YearLevel == FilterYearLevel.Value);
        }
        
        if (FilterDepartmentId.HasValue)
        {
            query = query.Where(c => c.DepartmentId == FilterDepartmentId.Value);
        }
        
        Courses = new ObservableCollection<Course>(query.OrderBy(c => c.YearLevel).ThenBy(c => c.Code).ToList());
    }
    
    private void LoadDepartments()
    {
        using var context = new UniversityDbContext();
        Departments = new ObservableCollection<Department>(context.Departments.OrderBy(d => d.Name).ToList());
        
        if (Departments.Any())
            FormDepartmentId = Departments.First().Id;
    }
    
    private void LoadCourseToForm(Course course)
    {
        FormName = course.Name;
        FormCode = course.Code;
        FormCredits = course.Credits;
        FormYearLevel = course.YearLevel;
        FormDepartmentId = course.DepartmentId;
        FormDescription = course.Description ?? string.Empty;
    }
    
    private void ClearForm()
    {
        FormName = string.Empty;
        FormCode = string.Empty;
        FormCredits = 3;
        FormYearLevel = 1;
        FormDepartmentId = Departments.FirstOrDefault()?.Id ?? 0;
        FormDescription = string.Empty;
    }
    
    private void StartAdd()
    {
        ClearForm();
        IsAdding = true;
        IsEditing = true;
        SelectedCourse = null;
    }
    
    private void StartEdit()
    {
        if (SelectedCourse != null)
        {
            IsAdding = false;
            IsEditing = true;
        }
    }
    
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(FormName) || string.IsNullOrWhiteSpace(FormCode))
        {
            MessageBox.Show("Name and Code are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        if (IsAdding)
        {
            // Check for duplicate code
            if (context.Courses.Any(c => c.Code == FormCode))
            {
                MessageBox.Show("A course with this code already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var course = new Course
            {
                Name = FormName,
                Code = FormCode,
                Credits = FormCredits,
                YearLevel = FormYearLevel,
                DepartmentId = FormDepartmentId,
                Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription
            };
            context.Courses.Add(course);
        }
        else if (SelectedCourse != null)
        {
            var course = context.Courses.Find(SelectedCourse.Id);
            if (course != null)
            {
                // Check for duplicate code (excluding current)
                if (context.Courses.Any(c => c.Code == FormCode && c.Id != course.Id))
                {
                    MessageBox.Show("A course with this code already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                course.Name = FormName;
                course.Code = FormCode;
                course.Credits = FormCredits;
                course.YearLevel = FormYearLevel;
                course.DepartmentId = FormDepartmentId;
                course.Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription;
            }
        }
        
        context.SaveChanges();
        IsEditing = false;
        IsAdding = false;
        LoadCourses();
    }
    
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        if (SelectedCourse != null)
            LoadCourseToForm(SelectedCourse);
        else
            ClearForm();
    }
    
    private void Delete()
    {
        if (SelectedCourse == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete {SelectedCourse.Name}?\nThis will also delete all associated sections and grades.", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var course = context.Courses.Find(SelectedCourse.Id);
            if (course != null)
            {
                context.Courses.Remove(course);
                context.SaveChanges();
            }
            LoadCourses();
            ClearForm();
        }
    }
    
    private void ClearFilter()
    {
        FilterYearLevel = null;
        FilterDepartmentId = null;
    }
}


