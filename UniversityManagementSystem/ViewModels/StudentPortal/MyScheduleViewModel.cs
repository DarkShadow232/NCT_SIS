using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.StudentPortal;

public class ScheduleItem
{
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string SectionName { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public string Schedule { get; set; } = "";
    public int YearLevel { get; set; }
    public int CreditHours { get; set; }
}

public class UpcomingExam
{
    public string Name { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string CourseCode { get; set; } = "";
    public DateTime ExamDate { get; set; }
    public string Room { get; set; } = "";
    public string Type { get; set; } = "";
    public string TypeDisplay => Type;
    public string DurationDisplay { get; set; } = "";
}

public class MyScheduleViewModel : BaseViewModel
{
    private ObservableCollection<ScheduleItem> _scheduleItems = new();
    private ObservableCollection<UpcomingExam> _upcomingExams = new();
    private int _totalCreditHours;
    private string _currentSemester = "Fall 2024-2025";
    
    public ObservableCollection<ScheduleItem> ScheduleItems
    {
        get => _scheduleItems;
        set => SetProperty(ref _scheduleItems, value);
    }
    
    public ObservableCollection<UpcomingExam> UpcomingExams
    {
        get => _upcomingExams;
        set => SetProperty(ref _upcomingExams, value);
    }
    
    public int TotalCreditHours
    {
        get => _totalCreditHours;
        set => SetProperty(ref _totalCreditHours, value);
    }
    
    public string CurrentSemester
    {
        get => _currentSemester;
        set => SetProperty(ref _currentSemester, value);
    }
    
    public MyScheduleViewModel()
    {
        LoadSchedule();
    }
    
    private void LoadSchedule()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.StudentId == null) return;
        
        using var context = new UniversityDbContext();
        
        var student = context.Students
            .Include(s => s.Section)
            .FirstOrDefault(s => s.Id == user.StudentId);
        
        if (student == null) return;
        
        // Get courses for student's year
        var courses = context.Courses
            .Where(c => c.YearLevel == student.YearLevel)
            .ToList();
        
        var scheduleItems = new List<ScheduleItem>();
        var random = new Random();
        string[] days = { "Sun-Tue", "Mon-Wed", "Tue-Thu" };
        string[] times = { "8:00 AM", "10:00 AM", "12:00 PM", "2:00 PM" };
        
        foreach (var course in courses)
        {
            var instructor = context.CourseInstructors
                .Include(ci => ci.Instructor)
                .FirstOrDefault(ci => ci.CourseId == course.Id);
            
            scheduleItems.Add(new ScheduleItem
            {
                CourseCode = course.Code,
                CourseName = course.Name,
                SectionName = student.Section?.Name ?? "A",
                InstructorName = instructor?.Instructor?.Name ?? "TBA",
                Schedule = $"{days[random.Next(days.Length)]} {times[random.Next(times.Length)]}",
                YearLevel = course.YearLevel,
                CreditHours = course.Credits
            });
        }
        
        ScheduleItems = new ObservableCollection<ScheduleItem>(scheduleItems);
        TotalCreditHours = scheduleItems.Sum(s => s.CreditHours);
        
        // Load upcoming exams
        var exams = context.Exams
            .Include(e => e.Course)
            .Where(e => e.IsActive && e.ExamDate >= DateTime.Today && e.Course!.YearLevel == student.YearLevel)
            .OrderBy(e => e.ExamDate)
            .Take(5)
            .ToList();
        
        UpcomingExams = new ObservableCollection<UpcomingExam>(
            exams.Select(e => new UpcomingExam
            {
                Name = e.Name,
                CourseName = e.Course?.Name ?? "",
                CourseCode = e.Course?.Code ?? "",
                ExamDate = e.ExamDate,
                Room = e.Room,
                Type = e.Type.ToString(),
                DurationDisplay = $"{e.DurationMinutes} min"
            }));
    }
}

