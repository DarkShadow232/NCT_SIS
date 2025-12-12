using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.ViewModels;

public class SectionsViewModel : BaseViewModel
{
    private ObservableCollection<Section> _sections = new();
    private ObservableCollection<Course> _courses = new();
    private ObservableCollection<Student> _sectionStudents = new();
    private Section? _selectedSection;
    private int? _filterCourseId;
    private bool _isEditing;
    private bool _isAdding;
    
    // Form fields
    private string _formName = string.Empty;
    private int _formCapacity = 30;
    private int _formCourseId;
    private string _formSchedule = string.Empty;
    private string _formRoom = string.Empty;
    
    public ObservableCollection<Section> Sections
    {
        get => _sections;
        set => SetProperty(ref _sections, value);
    }
    
    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }
    
    public ObservableCollection<Student> SectionStudents
    {
        get => _sectionStudents;
        set => SetProperty(ref _sectionStudents, value);
    }
    
    public Section? SelectedSection
    {
        get => _selectedSection;
        set
        {
            if (SetProperty(ref _selectedSection, value))
            {
                if (value != null && !_isAdding)
                {
                    LoadSectionToForm(value);
                    LoadSectionStudents(value.Id);
                }
                else
                {
                    SectionStudents.Clear();
                }
            }
        }
    }
    
    public int? FilterCourseId
    {
        get => _filterCourseId;
        set
        {
            if (SetProperty(ref _filterCourseId, value))
                LoadSections();
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
    
    public int FormCapacity
    {
        get => _formCapacity;
        set => SetProperty(ref _formCapacity, value);
    }
    
    public int FormCourseId
    {
        get => _formCourseId;
        set => SetProperty(ref _formCourseId, value);
    }
    
    public string FormSchedule
    {
        get => _formSchedule;
        set => SetProperty(ref _formSchedule, value);
    }
    
    public string FormRoom
    {
        get => _formRoom;
        set => SetProperty(ref _formRoom, value);
    }
    
    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    
    public SectionsViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedSection != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedSection != null);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        
        LoadCourses();
        LoadSections();
    }
    
    private void LoadSections()
    {
        using var context = new UniversityDbContext();
        var query = context.Sections
            .Include(s => s.Course)
            .Include(s => s.Students)
            .AsQueryable();
        
        if (FilterCourseId.HasValue)
        {
            query = query.Where(s => s.CourseId == FilterCourseId.Value);
        }
        
        Sections = new ObservableCollection<Section>(query.OrderBy(s => s.Course!.Code).ThenBy(s => s.Name).ToList());
    }
    
    private void LoadCourses()
    {
        using var context = new UniversityDbContext();
        Courses = new ObservableCollection<Course>(context.Courses.OrderBy(c => c.Code).ToList());
        
        if (Courses.Any())
            FormCourseId = Courses.First().Id;
    }
    
    private void LoadSectionStudents(int sectionId)
    {
        using var context = new UniversityDbContext();
        SectionStudents = new ObservableCollection<Student>(
            context.Students.Where(s => s.SectionId == sectionId).OrderBy(s => s.Name).ToList()
        );
    }
    
    private void LoadSectionToForm(Section section)
    {
        FormName = section.Name;
        FormCapacity = section.Capacity;
        FormCourseId = section.CourseId;
        FormSchedule = section.Schedule ?? string.Empty;
        FormRoom = section.Room ?? string.Empty;
    }
    
    private void ClearForm()
    {
        FormName = string.Empty;
        FormCapacity = 30;
        FormCourseId = Courses.FirstOrDefault()?.Id ?? 0;
        FormSchedule = string.Empty;
        FormRoom = string.Empty;
    }
    
    private void StartAdd()
    {
        ClearForm();
        IsAdding = true;
        IsEditing = true;
        SelectedSection = null;
    }
    
    private void StartEdit()
    {
        if (SelectedSection != null)
        {
            IsAdding = false;
            IsEditing = true;
        }
    }
    
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(FormName))
        {
            MessageBox.Show("Section name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        if (IsAdding)
        {
            var section = new Section
            {
                Name = FormName,
                Capacity = FormCapacity,
                CourseId = FormCourseId,
                Schedule = string.IsNullOrWhiteSpace(FormSchedule) ? null : FormSchedule,
                Room = string.IsNullOrWhiteSpace(FormRoom) ? null : FormRoom
            };
            context.Sections.Add(section);
        }
        else if (SelectedSection != null)
        {
            var section = context.Sections.Find(SelectedSection.Id);
            if (section != null)
            {
                section.Name = FormName;
                section.Capacity = FormCapacity;
                section.CourseId = FormCourseId;
                section.Schedule = string.IsNullOrWhiteSpace(FormSchedule) ? null : FormSchedule;
                section.Room = string.IsNullOrWhiteSpace(FormRoom) ? null : FormRoom;
            }
        }
        
        context.SaveChanges();
        IsEditing = false;
        IsAdding = false;
        LoadSections();
    }
    
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        if (SelectedSection != null)
            LoadSectionToForm(SelectedSection);
        else
            ClearForm();
    }
    
    private void Delete()
    {
        if (SelectedSection == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete section {SelectedSection.Name}?\nStudents in this section will be unassigned.", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var section = context.Sections.Find(SelectedSection.Id);
            if (section != null)
            {
                context.Sections.Remove(section);
                context.SaveChanges();
            }
            LoadSections();
            ClearForm();
            SectionStudents.Clear();
        }
    }
    
    private void ClearFilter()
    {
        FilterCourseId = null;
    }
}


