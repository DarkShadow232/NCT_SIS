using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Services;
using UniversityManagementSystem.ViewModels.StudentPortal;
using UniversityManagementSystem.ViewModels.InstructorPortal;
using UniversityManagementSystem.Views;

namespace UniversityManagementSystem.ViewModels;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel? _currentViewModel;
    private string _currentViewTitle = "Dashboard";
    private readonly AuthenticationService _authService = AuthenticationService.Instance;
    
    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }
    
    public string CurrentViewTitle
    {
        get => _currentViewTitle;
        set => SetProperty(ref _currentViewTitle, value);
    }
    
    // Role-based visibility
    public bool IsAdmin => _authService.IsAdmin;
    public bool IsStudent => _authService.IsStudent;
    public bool IsInstructor => _authService.IsInstructor;
    public string UserDisplayName => _authService.CurrentUser?.DisplayName ?? "User";
    public string UserRole => _authService.CurrentUser?.RoleDisplay ?? "User";
    
    // Admin Navigation Commands
    public ICommand NavigateToDashboardCommand { get; }
    public ICommand NavigateToStudentsCommand { get; }
    public ICommand NavigateToDepartmentsCommand { get; }
    public ICommand NavigateToCoursesCommand { get; }
    public ICommand NavigateToSectionsCommand { get; }
    public ICommand NavigateToFeesCommand { get; }
    public ICommand NavigateToGradesCommand { get; }
    public ICommand NavigateToInstructorsCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToUsersCommand { get; }
    public ICommand NavigateToAnnouncementsCommand { get; }
    
    // Student Portal Navigation Commands
    public ICommand NavigateToStudentDashboardCommand { get; }
    public ICommand NavigateToMyGradesCommand { get; }
    public ICommand NavigateToMyScheduleCommand { get; }
    public ICommand NavigateToMyFeesCommand { get; }
    public ICommand NavigateToStudentAnnouncementsCommand { get; }
    
    // Instructor Portal Navigation Commands
    public ICommand NavigateToInstructorDashboardCommand { get; }
    public ICommand NavigateToMyCoursesCommand { get; }
    public ICommand NavigateToGradeEntryCommand { get; }
    
    // Logout Command
    public ICommand LogoutCommand { get; }
    
    public MainViewModel()
    {
        // Admin commands
        NavigateToDashboardCommand = new RelayCommand(() => NavigateTo("Dashboard"));
        NavigateToStudentsCommand = new RelayCommand(() => NavigateTo("Students"));
        NavigateToDepartmentsCommand = new RelayCommand(() => NavigateTo("Departments"));
        NavigateToCoursesCommand = new RelayCommand(() => NavigateTo("Courses"));
        NavigateToSectionsCommand = new RelayCommand(() => NavigateTo("Sections"));
        NavigateToFeesCommand = new RelayCommand(() => NavigateTo("Fees"));
        NavigateToGradesCommand = new RelayCommand(() => NavigateTo("Grades"));
        NavigateToInstructorsCommand = new RelayCommand(() => NavigateTo("Instructors"));
        NavigateToReportsCommand = new RelayCommand(() => NavigateTo("Reports"));
        NavigateToUsersCommand = new RelayCommand(() => NavigateTo("Users"));
        NavigateToAnnouncementsCommand = new RelayCommand(() => NavigateTo("Announcements"));
        
        // Student Portal commands
        NavigateToStudentDashboardCommand = new RelayCommand(() => NavigateTo("StudentDashboard"));
        NavigateToMyGradesCommand = new RelayCommand(() => NavigateTo("MyGrades"));
        NavigateToMyScheduleCommand = new RelayCommand(() => NavigateTo("MySchedule"));
        NavigateToMyFeesCommand = new RelayCommand(() => NavigateTo("MyFees"));
        NavigateToStudentAnnouncementsCommand = new RelayCommand(() => NavigateTo("StudentAnnouncements"));
        
        // Instructor Portal commands
        NavigateToInstructorDashboardCommand = new RelayCommand(() => NavigateTo("InstructorDashboard"));
        NavigateToMyCoursesCommand = new RelayCommand(() => NavigateTo("MyCourses"));
        NavigateToGradeEntryCommand = new RelayCommand(() => NavigateTo("GradeEntry"));
        
        // Logout command
        LogoutCommand = new RelayCommand(Logout);
        
        // Start with appropriate dashboard based on role
        if (IsAdmin)
            NavigateTo("Dashboard");
        else if (IsStudent)
            NavigateTo("StudentDashboard");
        else if (IsInstructor)
            NavigateTo("InstructorDashboard");
        else
            NavigateTo("Dashboard");
    }
    
    private void NavigateTo(string viewName)
    {
        CurrentViewTitle = viewName switch
        {
            "StudentDashboard" => "My Dashboard",
            "MyGrades" => "My Grades",
            "MySchedule" => "My Schedule",
            "MyFees" => "My Fees",
            "StudentAnnouncements" => "Announcements",
            "InstructorDashboard" => "Dashboard",
            "MyCourses" => "My Courses",
            "GradeEntry" => "Grade Entry",
            _ => viewName
        };
        
        CurrentViewModel = viewName switch
        {
            // Admin views
            "Dashboard" => new DashboardViewModel(),
            "Students" => new StudentsViewModel(),
            "Departments" => new DepartmentsViewModel(),
            "Courses" => new CoursesViewModel(),
            "Sections" => new SectionsViewModel(),
            "Fees" => new FeesViewModel(),
            "Grades" => new GradesViewModel(),
            "Instructors" => new InstructorsViewModel(new UniversityDbContext()),
            "Reports" => new ReportsViewModel(),
            "Users" => new UsersViewModel(),
            "Announcements" => new AnnouncementsViewModel(),
            
            // Student Portal views
            "StudentDashboard" => new StudentDashboardViewModel(),
            "MyGrades" => new MyGradesViewModel(),
            "MySchedule" => new MyScheduleViewModel(),
            "MyFees" => new MyFeesViewModel(),
            "StudentAnnouncements" => new StudentAnnouncementsViewModel(),
            
            // Instructor Portal views
            "InstructorDashboard" => new InstructorDashboardViewModel(),
            "MyCourses" => new MyCoursesViewModel(),
            "GradeEntry" => new GradeEntryViewModel(),
            
            _ => new DashboardViewModel()
        };
    }
    
    private void Logout()
    {
        _authService.Logout();
        
        // Open login window and close current main window
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        
        Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
    }
}


