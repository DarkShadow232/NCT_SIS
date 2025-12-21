using System.IO;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem;

/// <summary>
/// Utility class to test database connectivity and data integrity
/// </summary>
public static class DatabaseTester
{
    public static string TestDatabase()
    {
        var results = new System.Text.StringBuilder();
        results.AppendLine("=== Database Connection Test ===\n");
        
        try
        {
            using var context = new UniversityDbContext();
            
            // Test 1: Database Connection
            results.AppendLine("✓ Test 1: Database Connection");
            var canConnect = context.Database.CanConnect();
            results.AppendLine($"   Result: {(canConnect ? "SUCCESS" : "FAILED")}");
            results.AppendLine($"   Database Path: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "university.db")}");
            results.AppendLine();
            
            if (!canConnect)
            {
                results.AppendLine("❌ Cannot connect to database!");
                return results.ToString();
            }
            
            // Test 2: Check Tables Exist (by trying to query them)
            results.AppendLine("✓ Test 2: Check Tables Exist");
            try
            {
                _ = context.Departments.Any();
                results.AppendLine("   Departments: ✓ EXISTS");
            }
            catch { results.AppendLine("   Departments: ✗ MISSING"); }
            
            try
            {
                _ = context.Courses.Any();
                results.AppendLine("   Courses: ✓ EXISTS");
            }
            catch { results.AppendLine("   Courses: ✗ MISSING"); }
            
            try
            {
                _ = context.Students.Any();
                results.AppendLine("   Students: ✓ EXISTS");
            }
            catch { results.AppendLine("   Students: ✗ MISSING"); }
            
            try
            {
                _ = context.Grades.Any();
                results.AppendLine("   Grades: ✓ EXISTS");
            }
            catch { results.AppendLine("   Grades: ✗ MISSING"); }
            
            try
            {
                _ = context.Instructors.Any();
                results.AppendLine("   Instructors: ✓ EXISTS");
            }
            catch { results.AppendLine("   Instructors: ✗ MISSING"); }
            
            try
            {
                _ = context.Attendances.Any();
                results.AppendLine("   Attendances: ✓ EXISTS");
            }
            catch { results.AppendLine("   Attendances: ✗ MISSING"); }
            
            try
            {
                _ = context.LectureHalls.Any();
                results.AppendLine("   LectureHalls: ✓ EXISTS");
            }
            catch { results.AppendLine("   LectureHalls: ✗ MISSING"); }
            
            try
            {
                _ = context.Laboratories.Any();
                results.AppendLine("   Laboratories: ✓ EXISTS");
            }
            catch { results.AppendLine("   Laboratories: ✗ MISSING"); }
            
            try
            {
                _ = context.AcademicYears.Any();
                results.AppendLine("   AcademicYears: ✓ EXISTS");
            }
            catch { results.AppendLine("   AcademicYears: ✗ MISSING"); }
            
            try
            {
                _ = context.AcademicCalendars.Any();
                results.AppendLine("   AcademicCalendars: ✓ EXISTS");
            }
            catch { results.AppendLine("   AcademicCalendars: ✗ MISSING"); }
            
            try
            {
                _ = context.Users.Any();
                results.AppendLine("   Users: ✓ EXISTS");
            }
            catch { results.AppendLine("   Users: ✗ MISSING"); }
            
            results.AppendLine();
            
            // Test 3: Count Records
            results.AppendLine("✓ Test 3: Count Records");
            results.AppendLine($"   Departments: {context.Departments.Count()}");
            results.AppendLine($"   Courses: {context.Courses.Count()}");
            results.AppendLine($"   Students: {context.Students.Count()}");
            results.AppendLine($"   Grades: {context.Grades.Count()}");
            results.AppendLine($"   Instructors: {context.Instructors.Count()}");
            results.AppendLine($"   Users: {context.Users.Count()}");
            results.AppendLine($"   Attendances: {context.Attendances.Count()}");
            results.AppendLine($"   LectureHalls: {context.LectureHalls.Count()}");
            results.AppendLine($"   Laboratories: {context.Laboratories.Count()}");
            results.AppendLine($"   AcademicYears: {context.AcademicYears.Count()}");
            results.AppendLine($"   AcademicCalendars: {context.AcademicCalendars.Count()}");
            results.AppendLine();
            
            // Test 4: Test Relationships
            results.AppendLine("✓ Test 4: Test Relationships");
            var studentWithGrades = context.Students
                .Include(s => s.Grades)
                .Include(s => s.Department)
                .FirstOrDefault();
            
            if (studentWithGrades != null)
            {
                results.AppendLine($"   Student '{studentWithGrades.Name}' has:");
                results.AppendLine($"     - Department: {(studentWithGrades.Department != null ? studentWithGrades.Department.Name : "NULL")}");
                results.AppendLine($"     - Grades: {studentWithGrades.Grades.Count}");
            }
            else
            {
                results.AppendLine("   ⚠ No students found");
            }
            
            var courseWithGrades = context.Courses
                .Include(c => c.Grades)
                .Include(c => c.Department)
                .FirstOrDefault();
            
            if (courseWithGrades != null)
            {
                results.AppendLine($"   Course '{courseWithGrades.Name}' has:");
                results.AppendLine($"     - Department: {(courseWithGrades.Department != null ? courseWithGrades.Department.Name : "NULL")}");
                results.AppendLine($"     - Grades: {courseWithGrades.Grades.Count}");
            }
            else
            {
                results.AppendLine("   ⚠ No courses found");
            }
            results.AppendLine();
            
            // Test 5: Test Grade Calculations
            results.AppendLine("✓ Test 5: Test Grade Calculations");
            var gradeWithCourse = context.Grades
                .Include(g => g.Course)
                .Include(g => g.Student)
                .FirstOrDefault(g => g.Course != null);
            
            if (gradeWithCourse != null && gradeWithCourse.Course != null)
            {
                results.AppendLine($"   Grade for '{gradeWithCourse.Student?.Name}' in '{gradeWithCourse.Course.Name}':");
                results.AppendLine($"     - Assignment1: {gradeWithCourse.Assignment1}");
                results.AppendLine($"     - Assignment2: {gradeWithCourse.Assignment2}");
                results.AppendLine($"     - CourseWork: {gradeWithCourse.CourseWork}");
                results.AppendLine($"     - FinalExam: {gradeWithCourse.FinalExam}");
                results.AppendLine($"     - TotalScore: {gradeWithCourse.TotalScore}");
                results.AppendLine($"     - SymbolicGrade: {gradeWithCourse.SymbolicGrade}");
                results.AppendLine($"     - CourseType: {gradeWithCourse.Course.CourseType}");
            }
            else
            {
                results.AppendLine("   ⚠ No grades with courses found");
            }
            results.AppendLine();
            
            // Test 6: Test Users
            results.AppendLine("✓ Test 6: Test Users");
            var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (adminUser != null)
            {
                results.AppendLine($"   Admin user exists: ✓");
                results.AppendLine($"   Role: {adminUser.Role}");
                results.AppendLine($"   IsActive: {adminUser.IsActive}");
            }
            else
            {
                results.AppendLine("   ⚠ Admin user not found");
            }
            results.AppendLine();
            
            results.AppendLine("=== All Tests Completed Successfully! ===");
        }
        catch (Exception ex)
        {
            results.AppendLine($"❌ ERROR: {ex.Message}");
            results.AppendLine($"Stack Trace: {ex.StackTrace}");
        }
        
        return results.ToString();
    }
}

