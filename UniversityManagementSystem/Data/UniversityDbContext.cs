using System.IO;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Data;

public class UniversityDbContext : DbContext
{
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;
    public DbSet<StudentFee> StudentFees { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Instructor> Instructors { get; set; } = null!;
    public DbSet<CourseInstructor> CourseInstructors { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<Announcement> Announcements { get; set; } = null!;
    
    // New models based on PDF requirements
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<LectureHall> LectureHalls { get; set; } = null!;
    public DbSet<Laboratory> Laboratories { get; set; } = null!;
    public DbSet<Specification> Specifications { get; set; } = null!;
    public DbSet<AcademicYear> AcademicYears { get; set; } = null!;
    public DbSet<AcademicCalendar> AcademicCalendars { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "university.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AnnualFees).HasColumnType("decimal(18,2)");
            entity.HasMany(e => e.Courses)
                  .WithOne(c => c.Department)
                  .HasForeignKey(c => c.DepartmentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Students)
                  .WithOne(s => s.Department)
                  .HasForeignKey(s => s.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasMany(e => e.Sections)
                  .WithOne(s => s.Course)
                  .HasForeignKey(s => s.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Grades)
                  .WithOne(g => g.Course)
                  .HasForeignKey(g => g.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Section configuration
        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasMany(e => e.Students)
                  .WithOne(s => s.Section)
                  .HasForeignKey(s => s.SectionId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.StudentId).IsUnique();
            entity.HasMany(e => e.Grades)
                  .WithOne(g => g.Student)
                  .HasForeignKey(g => g.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Fees)
                  .WithOne(f => f.Student)
                  .HasForeignKey(f => f.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Grade configuration
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        });
        
        // StudentFee configuration
        modelBuilder.Entity<StudentFee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => new { e.StudentId, e.AcademicYear }).IsUnique();
        });
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasOne(e => e.Student)
                  .WithMany()
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Instructor)
                  .WithMany()
                  .HasForeignKey(e => e.InstructorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Instructor configuration
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // CourseInstructor configuration
        modelBuilder.Entity<CourseInstructor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Instructor)
                  .WithMany(i => i.CourseInstructors)
                  .HasForeignKey(e => e.InstructorId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Course)
                  .WithMany()
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Exam configuration
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Course)
                  .WithMany()
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Announcement configuration
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Attendance configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                  .WithMany()
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Course)
                  .WithMany()
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Section)
                  .WithMany(s => s.Attendances)
                  .HasForeignKey(e => e.SectionId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.Date });
        });
        
        // LectureHall configuration
        modelBuilder.Entity<LectureHall>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Sections)
                  .WithOne(s => s.LectureHall)
                  .HasForeignKey(s => s.LectureHallId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Laboratory configuration
        modelBuilder.Entity<Laboratory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.LabSections)
                  .WithOne(s => s.Laboratory)
                  .HasForeignKey(s => s.LaboratoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Specification configuration
        modelBuilder.Entity<Specification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ProductId).IsUnique();
        });
        
        // AcademicYear configuration
        modelBuilder.Entity<AcademicYear>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Year).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Year).IsUnique();
            entity.HasMany(e => e.CalendarEvents)
                  .WithOne(c => c.AcademicYear)
                  .HasForeignKey(c => c.AcademicYearId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // AcademicCalendar configuration
        modelBuilder.Entity<AcademicCalendar>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });
    }
}
