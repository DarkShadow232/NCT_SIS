using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.ViewModels;

public class AcademicCalendarViewModel : BaseViewModel
{
    private ObservableCollection<AcademicYear> _academicYears = new();
    private ObservableCollection<AcademicCalendar> _calendarEvents = new();
    private AcademicYear? _selectedAcademicYear;
    private AcademicCalendar? _selectedEvent;
    private bool _isEditingYear;
    private bool _isEditingEvent;
    
    // Academic Year Form fields
    private string _yearName = string.Empty;
    private string _semester = string.Empty;
    private DateTime _yearStartDate = DateTime.Today;
    private DateTime _yearEndDate = DateTime.Today.AddMonths(10);
    private bool _isActiveYear = false;
    private string _yearDescription = string.Empty;
    
    // Event Form fields
    private string _eventTitle = string.Empty;
    private string _eventDescription = string.Empty;
    private DateTime _eventDate = DateTime.Today;
    private DateTime? _eventEndDate;
    private string _eventType = "Lecture";
    private bool _isAllDay = true;
    
    public ObservableCollection<AcademicYear> AcademicYears
    {
        get => _academicYears;
        set => SetProperty(ref _academicYears, value);
    }
    
    public ObservableCollection<AcademicCalendar> CalendarEvents
    {
        get => _calendarEvents;
        set => SetProperty(ref _calendarEvents, value);
    }
    
    public AcademicYear? SelectedAcademicYear
    {
        get => _selectedAcademicYear;
        set
        {
            if (SetProperty(ref _selectedAcademicYear, value))
                LoadCalendarEvents();
        }
    }
    
    public AcademicCalendar? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            if (SetProperty(ref _selectedEvent, value) && value != null)
            {
                LoadEventToForm(value);
            }
        }
    }
    
    public bool IsEditingYear
    {
        get => _isEditingYear;
        set => SetProperty(ref _isEditingYear, value);
    }
    
    public bool IsEditingEvent
    {
        get => _isEditingEvent;
        set => SetProperty(ref _isEditingEvent, value);
    }
    
    public string YearName
    {
        get => _yearName;
        set => SetProperty(ref _yearName, value);
    }
    
    public string Semester
    {
        get => _semester;
        set => SetProperty(ref _semester, value);
    }
    
    public DateTime YearStartDate
    {
        get => _yearStartDate;
        set => SetProperty(ref _yearStartDate, value);
    }
    
    public DateTime YearEndDate
    {
        get => _yearEndDate;
        set => SetProperty(ref _yearEndDate, value);
    }
    
    public bool IsActiveYear
    {
        get => _isActiveYear;
        set => SetProperty(ref _isActiveYear, value);
    }
    
    public string YearDescription
    {
        get => _yearDescription;
        set => SetProperty(ref _yearDescription, value);
    }
    
    public string EventTitle
    {
        get => _eventTitle;
        set => SetProperty(ref _eventTitle, value);
    }
    
    public string EventDescription
    {
        get => _eventDescription;
        set => SetProperty(ref _eventDescription, value);
    }
    
    public DateTime EventDate
    {
        get => _eventDate;
        set => SetProperty(ref _eventDate, value);
    }
    
    public DateTime? EventEndDate
    {
        get => _eventEndDate;
        set => SetProperty(ref _eventEndDate, value);
    }
    
    public string EventType
    {
        get => _eventType;
        set => SetProperty(ref _eventType, value);
    }
    
    public bool IsAllDay
    {
        get => _isAllDay;
        set => SetProperty(ref _isAllDay, value);
    }
    
    public ObservableCollection<string> EventTypes { get; } = new()
    {
        "Exam", "Holiday", "Registration", "Lecture", "Orientation", "Deadline", "Meeting", "Other"
    };
    
    public ObservableCollection<string> Semesters { get; } = new()
    {
        "First Semester", "Second Semester", "Summer Semester"
    };
    
    public ICommand AddYearCommand { get; }
    public ICommand SaveYearCommand { get; }
    public ICommand CancelYearCommand { get; }
    public ICommand DeleteYearCommand { get; }
    public ICommand AddEventCommand { get; }
    public ICommand SaveEventCommand { get; }
    public ICommand CancelEventCommand { get; }
    public ICommand DeleteEventCommand { get; }
    public ICommand RefreshCommand { get; }
    
    public AcademicCalendarViewModel()
    {
        AddYearCommand = new RelayCommand(StartAddYear);
        SaveYearCommand = new RelayCommand(SaveYear);
        CancelYearCommand = new RelayCommand(CancelYear);
        DeleteYearCommand = new RelayCommand(DeleteYear, () => SelectedAcademicYear != null);
        AddEventCommand = new RelayCommand(StartAddEvent, () => SelectedAcademicYear != null);
        SaveEventCommand = new RelayCommand(SaveEvent);
        CancelEventCommand = new RelayCommand(CancelEvent);
        DeleteEventCommand = new RelayCommand(DeleteEvent, () => SelectedEvent != null);
        RefreshCommand = new RelayCommand(LoadData);
        
        LoadData();
    }
    
    private void LoadData()
    {
        LoadAcademicYears();
    }
    
    private void LoadAcademicYears()
    {
        using var context = new UniversityDbContext();
        var years = context.AcademicYears
            .Include(y => y.CalendarEvents)
            .OrderByDescending(y => y.Year)
            .ToList();
        AcademicYears = new ObservableCollection<AcademicYear>(years);
    }
    
    private void LoadCalendarEvents()
    {
        if (SelectedAcademicYear == null)
        {
            CalendarEvents = new ObservableCollection<AcademicCalendar>();
            return;
        }
        
        using var context = new UniversityDbContext();
        var events = context.AcademicCalendars
            .Where(e => e.AcademicYearId == SelectedAcademicYear.Id)
            .OrderBy(e => e.EventDate)
            .ToList();
        CalendarEvents = new ObservableCollection<AcademicCalendar>(events);
    }
    
    private void StartAddYear()
    {
        IsEditingYear = true;
        ClearYearForm();
    }
    
    private void SaveYear()
    {
        if (string.IsNullOrWhiteSpace(YearName))
        {
            MessageBox.Show("Please enter an academic year.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        var year = new AcademicYear
        {
            Year = YearName,
            Semester = Semester,
            StartDate = YearStartDate,
            EndDate = YearEndDate,
            IsActive = IsActiveYear,
            Description = YearDescription
        };
        
        context.AcademicYears.Add(year);
        context.SaveChanges();
        
        LoadAcademicYears();
        IsEditingYear = false;
        ClearYearForm();
        
        MessageBox.Show("Academic Year saved successfully!", "Success", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void CancelYear()
    {
        IsEditingYear = false;
        ClearYearForm();
    }
    
    private void DeleteYear()
    {
        if (SelectedAcademicYear == null) return;
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete academic year '{SelectedAcademicYear.Year}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var year = context.AcademicYears.Find(SelectedAcademicYear.Id);
        if (year != null)
        {
            context.AcademicYears.Remove(year);
            context.SaveChanges();
            LoadAcademicYears();
            MessageBox.Show("Academic Year deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void StartAddEvent()
    {
        if (SelectedAcademicYear == null)
        {
            MessageBox.Show("Please select an academic year first.", "Information", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        
        IsEditingEvent = true;
        ClearEventForm();
    }
    
    private void SaveEvent()
    {
        if (SelectedAcademicYear == null || string.IsNullOrWhiteSpace(EventTitle))
        {
            MessageBox.Show("Please select an academic year and enter event title.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        var calendarEvent = new AcademicCalendar
        {
            AcademicYearId = SelectedAcademicYear.Id,
            Title = EventTitle,
            Description = EventDescription,
            EventDate = EventDate,
            EndDate = EventEndDate,
            EventType = EventType,
            IsAllDay = IsAllDay
        };
        
        context.AcademicCalendars.Add(calendarEvent);
        context.SaveChanges();
        
        LoadCalendarEvents();
        IsEditingEvent = false;
        ClearEventForm();
        
        MessageBox.Show("Event saved successfully!", "Success", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void CancelEvent()
    {
        IsEditingEvent = false;
        ClearEventForm();
    }
    
    private void DeleteEvent()
    {
        if (SelectedEvent == null) return;
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete event '{SelectedEvent.Title}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var eventItem = context.AcademicCalendars.Find(SelectedEvent.Id);
        if (eventItem != null)
        {
            context.AcademicCalendars.Remove(eventItem);
            context.SaveChanges();
            LoadCalendarEvents();
            MessageBox.Show("Event deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void LoadEventToForm(AcademicCalendar eventItem)
    {
        EventTitle = eventItem.Title;
        EventDescription = eventItem.Description ?? string.Empty;
        EventDate = eventItem.EventDate;
        EventEndDate = eventItem.EndDate;
        EventType = eventItem.EventType ?? "Lecture";
        IsAllDay = eventItem.IsAllDay;
    }
    
    private void ClearYearForm()
    {
        YearName = string.Empty;
        Semester = string.Empty;
        YearStartDate = DateTime.Today;
        YearEndDate = DateTime.Today.AddMonths(10);
        IsActiveYear = false;
        YearDescription = string.Empty;
    }
    
    private void ClearEventForm()
    {
        EventTitle = string.Empty;
        EventDescription = string.Empty;
        EventDate = DateTime.Today;
        EventEndDate = null;
        EventType = "Lecture";
        IsAllDay = true;
    }
}

