using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class AnnouncementsViewModel : BaseViewModel
{
    private ObservableCollection<Announcement> _announcements = new();
    private Announcement? _selectedAnnouncement;
    private string _title = string.Empty;
    private string _content = string.Empty;
    private AnnouncementPriority _priority = AnnouncementPriority.Normal;
    private DateTime _publishDate = DateTime.Now;
    private DateTime? _expiryDate;
    private bool _isActive = true;
    private bool _isEditing;
    private string _errorMessage = string.Empty;
    
    public ObservableCollection<Announcement> Announcements
    {
        get => _announcements;
        set => SetProperty(ref _announcements, value);
    }
    
    public Announcement? SelectedAnnouncement
    {
        get => _selectedAnnouncement;
        set
        {
            SetProperty(ref _selectedAnnouncement, value);
            if (value != null)
            {
                Title = value.Title;
                Content = value.Content;
                Priority = value.Priority;
                PublishDate = value.PublishDate;
                ExpiryDate = value.ExpiryDate;
                IsActive = value.IsActive;
                IsEditing = true;
            }
        }
    }
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    
    public AnnouncementPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }
    
    public DateTime PublishDate
    {
        get => _publishDate;
        set => SetProperty(ref _publishDate, value);
    }
    
    public DateTime? ExpiryDate
    {
        get => _expiryDate;
        set => SetProperty(ref _expiryDate, value);
    }
    
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    
    public List<AnnouncementPriority> Priorities { get; } = 
        Enum.GetValues<AnnouncementPriority>().ToList();
    
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }
    
    public AnnouncementsViewModel()
    {
        AddCommand = new RelayCommand(AddAnnouncement);
        UpdateCommand = new RelayCommand(UpdateAnnouncement);
        DeleteCommand = new RelayCommand(DeleteAnnouncement);
        ClearCommand = new RelayCommand(ClearForm);
        
        LoadAnnouncements();
    }
    
    private void LoadAnnouncements()
    {
        using var context = new UniversityDbContext();
        var announcements = context.Announcements
            .Include(a => a.CreatedByUser)
            .OrderByDescending(a => a.PublishDate)
            .ToList();
        Announcements = new ObservableCollection<Announcement>(announcements);
    }
    
    private void AddAnnouncement()
    {
        ErrorMessage = string.Empty;
        
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Title is required";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Content))
        {
            ErrorMessage = "Content is required";
            return;
        }
        
        using var context = new UniversityDbContext();
        
        var announcement = new Announcement
        {
            Title = Title,
            Content = Content,
            Priority = Priority,
            PublishDate = PublishDate,
            ExpiryDate = ExpiryDate,
            IsActive = IsActive,
            CreatedByUserId = AuthenticationService.Instance.CurrentUser?.Id
        };
        
        context.Announcements.Add(announcement);
        context.SaveChanges();
        
        MessageBox.Show("Announcement created successfully!", "Success", 
            MessageBoxButton.OK, MessageBoxImage.Information);
        
        LoadAnnouncements();
        ClearForm();
    }
    
    private void UpdateAnnouncement()
    {
        ErrorMessage = string.Empty;
        
        if (SelectedAnnouncement == null)
        {
            ErrorMessage = "Please select an announcement to update";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Title is required";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Content))
        {
            ErrorMessage = "Content is required";
            return;
        }
        
        using var context = new UniversityDbContext();
        
        var announcement = context.Announcements.Find(SelectedAnnouncement.Id);
        if (announcement == null)
        {
            ErrorMessage = "Announcement not found";
            return;
        }
        
        announcement.Title = Title;
        announcement.Content = Content;
        announcement.Priority = Priority;
        announcement.PublishDate = PublishDate;
        announcement.ExpiryDate = ExpiryDate;
        announcement.IsActive = IsActive;
        
        context.SaveChanges();
        
        MessageBox.Show("Announcement updated successfully!", "Success", 
            MessageBoxButton.OK, MessageBoxImage.Information);
        
        LoadAnnouncements();
        ClearForm();
    }
    
    private void DeleteAnnouncement()
    {
        if (SelectedAnnouncement == null)
        {
            MessageBox.Show("Please select an announcement to delete", "Warning", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var result = MessageBox.Show($"Are you sure you want to delete '{SelectedAnnouncement.Title}'?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var announcement = context.Announcements.Find(SelectedAnnouncement.Id);
            if (announcement != null)
            {
                context.Announcements.Remove(announcement);
                context.SaveChanges();
            }
            
            LoadAnnouncements();
            ClearForm();
        }
    }
    
    private void ClearForm()
    {
        SelectedAnnouncement = null;
        Title = string.Empty;
        Content = string.Empty;
        Priority = AnnouncementPriority.Normal;
        PublishDate = DateTime.Now;
        ExpiryDate = null;
        IsActive = true;
        IsEditing = false;
        ErrorMessage = string.Empty;
    }
}

