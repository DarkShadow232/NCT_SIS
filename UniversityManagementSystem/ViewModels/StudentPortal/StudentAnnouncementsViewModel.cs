using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.ViewModels.StudentPortal;

public class StudentAnnouncementsViewModel : BaseViewModel
{
    private ObservableCollection<Announcement> _announcements = new();
    private Announcement? _selectedAnnouncement;
    private string? _filterPriority;
    
    public ObservableCollection<Announcement> Announcements
    {
        get => _announcements;
        set => SetProperty(ref _announcements, value);
    }
    
    public Announcement? SelectedAnnouncement
    {
        get => _selectedAnnouncement;
        set => SetProperty(ref _selectedAnnouncement, value);
    }
    
    public string? FilterPriority
    {
        get => _filterPriority;
        set { SetProperty(ref _filterPriority, value); FilterAnnouncements(); }
    }
    
    public List<string?> PriorityOptions { get; } = new() { null, "Normal", "Important", "Urgent" };
    
    public ICommand SelectCommand { get; }
    
    private List<Announcement> _allAnnouncements = new();
    
    public StudentAnnouncementsViewModel()
    {
        SelectCommand = new RelayCommand(a => SelectedAnnouncement = a as Announcement);
        LoadAnnouncements();
    }
    
    private void LoadAnnouncements()
    {
        using var context = new UniversityDbContext();
        
        _allAnnouncements = context.Announcements
            .Include(a => a.CreatedByUser)
            .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.PublishDate)
            .ToList();
        
        FilterAnnouncements();
    }
    
    private void FilterAnnouncements()
    {
        var filtered = _allAnnouncements.AsEnumerable();
        
        if (!string.IsNullOrEmpty(FilterPriority))
        {
            filtered = filtered.Where(a => a.Priority.ToString() == FilterPriority);
        }
        
        Announcements = new ObservableCollection<Announcement>(filtered);
        
        if (Announcements.Any() && SelectedAnnouncement == null)
        {
            SelectedAnnouncement = Announcements.First();
        }
    }
}

