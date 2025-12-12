namespace UniversityManagementSystem.Models;

public enum AnnouncementPriority
{
    Normal,
    Important,
    Urgent
}

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
    public DateTime PublishDate { get; set; } = DateTime.Now;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    
    public string PriorityDisplay => Priority.ToString();
    public string PublishDateDisplay => PublishDate.ToString("MMM dd, yyyy");
    public string? ExpiryDateDisplay => ExpiryDate?.ToString("MMM dd, yyyy");
    public string CreatedByName => CreatedByUser?.DisplayName ?? "System";
}

