using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;

namespace UniversityManagementSystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Add global exception handlers
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        try
        {
            // Initialize database
            using var context = new UniversityDbContext();
            
            // Check if database needs to be recreated for new schema
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "university.db");
            
            // Delete old database if it exists to recreate with new schema
            // This ensures all new tables (Attendance, LectureHall, etc.) are created
            if (File.Exists(dbPath))
            {
                try
                {
                    context.Database.EnsureDeleted();
                }
                catch
                {
                    // If can't delete, try to continue
                }
            }
            
            // Create database with all new tables
            context.Database.EnsureCreated();
            DbSeeder.SeedData(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database initialization error:\n{ex.Message}\n\n{ex.InnerException?.Message}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"An error occurred:\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}", 
            "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
    
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"Critical error:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
