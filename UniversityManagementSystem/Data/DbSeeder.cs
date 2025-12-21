using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.Data;

public static class DbSeeder
{
    public static void SeedData(UniversityDbContext context)
    {
        // Seed Users first (always check for users)
        SeedUsers(context);
        
        if (context.Departments.Any()) return;
        
        // Seed Departments with Annual Fees
        var departments = new List<Department>
        {
            new() { Code = "CS", Name = "Computer Science", HeadOfDepartment = "Dr. Ahmed Hassan", Description = "Department of Computer Science and Software Engineering", AnnualFees = 45000 },
            new() { Code = "IT", Name = "Information Technology", HeadOfDepartment = "Dr. Sara Mohamed", Description = "Department of Information Technology and Networks", AnnualFees = 42000 },
            new() { Code = "BUS", Name = "Business Administration", HeadOfDepartment = "Dr. Omar Khaled", Description = "Department of Business and Management", AnnualFees = 38000 },
            new() { Code = "ENG", Name = "Engineering", HeadOfDepartment = "Dr. Fatma Ali", Description = "Department of Engineering and Applied Sciences", AnnualFees = 55000 }
        };
        context.Departments.AddRange(departments);
        context.SaveChanges();
        
        // Seed Courses - fewer courses for faster loading
        var courses = new List<Course>
        {
            // Computer Science - Mix of Practical and Theoretical
            new() { Name = "Introduction to Programming", Code = "CS101", Credits = 4, YearLevel = 1, DepartmentId = departments[0].Id, CourseType = CourseType.Practical150, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Data Structures", Code = "CS201", Credits = 4, YearLevel = 2, DepartmentId = departments[0].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Software Engineering", Code = "CS301", Credits = 4, YearLevel = 3, DepartmentId = departments[0].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Machine Learning", Code = "CS401", Credits = 4, YearLevel = 4, DepartmentId = departments[0].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            
            // Information Technology - Mix of Practical and Theoretical
            new() { Name = "IT Fundamentals", Code = "IT101", Credits = 3, YearLevel = 1, DepartmentId = departments[1].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            new() { Name = "Web Development", Code = "IT201", Credits = 4, YearLevel = 2, DepartmentId = departments[1].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Network Administration", Code = "IT301", Credits = 4, YearLevel = 3, DepartmentId = departments[1].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Cybersecurity", Code = "IT401", Credits = 4, YearLevel = 4, DepartmentId = departments[1].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            
            // Business Administration - Mostly Theoretical
            new() { Name = "Introduction to Business", Code = "BUS101", Credits = 3, YearLevel = 1, DepartmentId = departments[2].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            new() { Name = "Marketing Fundamentals", Code = "BUS201", Credits = 3, YearLevel = 2, DepartmentId = departments[2].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            new() { Name = "Financial Management", Code = "BUS301", Credits = 4, YearLevel = 3, DepartmentId = departments[2].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            new() { Name = "Strategic Management", Code = "BUS401", Credits = 4, YearLevel = 4, DepartmentId = departments[2].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            
            // Engineering - Mix of Practical and Theoretical
            new() { Name = "Engineering Mathematics", Code = "ENG101", Credits = 4, YearLevel = 1, DepartmentId = departments[3].Id, CourseType = CourseType.Theoretical100, CurriculumStructure = CurriculumStructure.Theoretical },
            new() { Name = "Circuit Analysis", Code = "ENG201", Credits = 4, YearLevel = 2, DepartmentId = departments[3].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Digital Electronics", Code = "ENG301", Credits = 4, YearLevel = 3, DepartmentId = departments[3].Id, CourseType = CourseType.Practical150, CurriculumStructure = CurriculumStructure.Practical },
            new() { Name = "Control Systems", Code = "ENG401", Credits = 4, YearLevel = 4, DepartmentId = departments[3].Id, CourseType = CourseType.Practical100, CurriculumStructure = CurriculumStructure.Practical }
        };
        context.Courses.AddRange(courses);
        context.SaveChanges();
        
        // Seed Sections - 2 sections per course
        var sections = new List<Section>();
        foreach (var course in courses)
        {
            sections.Add(new Section 
            { 
                Name = $"Section 1", 
                CourseId = course.Id, 
                Capacity = 40, 
                Schedule = "Sun-Tue 9:00-10:30",
                Room = $"Room {100 + course.YearLevel}"
            });
            sections.Add(new Section 
            { 
                Name = $"Section 2", 
                CourseId = course.Id, 
                Capacity = 40, 
                Schedule = "Mon-Wed 11:00-12:30",
                Room = $"Room {200 + course.YearLevel}"
            });
        }
        context.Sections.AddRange(sections);
        context.SaveChanges();
        
        // Seed Students - reduced to 40 total (10 per department)
        var random = new Random(42);
        var firstNames = new[] { "Ahmed", "Mohamed", "Omar", "Youssef", "Ali", "Sara", "Fatma", "Nour", "Mariam", "Hana" };
        var lastNames = new[] { "Hassan", "Mohamed", "Ahmed", "Ali", "Ibrahim", "Khaled", "Mahmoud", "Youssef", "Salem", "Nasser" };
        
        var students = new List<Student>();
        int studentCounter = 1;
        
        foreach (var dept in departments)
        {
            var deptSections = sections.Where(s => courses.First(c => c.Id == s.CourseId).DepartmentId == dept.Id).ToList();
            
            for (int i = 0; i < 10; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var yearLevel = (i % 4) + 1;
                
                students.Add(new Student
                {
                    Name = $"{firstName} {lastName}",
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}{studentCounter}@nct.edu.eg",
                    StudentId = $"NCT{2024000 + studentCounter:D5}",
                    Phone = $"+20-10{random.Next(10000000, 99999999)}",
                    YearLevel = yearLevel,
                    DepartmentId = dept.Id,
                    EnrollmentDate = DateTime.Now.AddDays(-random.Next(30, 365)),
                    IsActive = true,
                    SectionId = deptSections.Count > 0 ? deptSections[random.Next(deptSections.Count)].Id : null
                });
                studentCounter++;
            }
        }
        context.Students.AddRange(students);
        context.SaveChanges();
        
        // Seed Student Fees
        var studentFees = new List<StudentFee>();
        foreach (var student in students)
        {
            var dept = departments.First(d => d.Id == student.DepartmentId);
            var isPaid = random.Next(100) > 40;
            
            studentFees.Add(new StudentFee
            {
                StudentId = student.Id,
                AcademicYear = student.YearLevel,
                Amount = dept.AnnualFees,
                AmountPaid = isPaid ? dept.AnnualFees : 0,
                Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Pending,
                AssignedDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                LastPaymentDate = isPaid ? DateTime.Now.AddDays(-random.Next(1, 30)) : null
            });
        }
        context.StudentFees.AddRange(studentFees);
        context.SaveChanges();
        
        // Seed Grades - with CourseWork field and proper calculations for all students
        var gradingService = new GradingService();
        var grades = new List<Grade>();
        
        foreach (var student in students)
        {
            var studentCourse = courses.FirstOrDefault(c => c.YearLevel == student.YearLevel && c.DepartmentId == student.DepartmentId);
            if (studentCourse != null)
            {
                var grade = new Grade
                {
                    StudentId = student.Id,
                    CourseId = studentCourse.Id,
                    Assignment1 = random.Next(60, 100),
                    Assignment2 = random.Next(60, 100),
                    CourseWork = random.Next(65, 100),  // NEW: Course Work field
                    FinalExam = studentCourse.CourseType == CourseType.Theoretical100 ? null : random.Next(55, 100),
                    GradedDate = DateTime.Now.AddDays(-random.Next(1, 15)),
                    Course = studentCourse  // Set course for calculation
                };
                
                // Calculate total score and symbolic grade
                gradingService.CalculateGrade(grade);
                
                grades.Add(grade);
            }
        }
        context.Grades.AddRange(grades);
        context.SaveChanges();
        
        // Seed Instructors (Doctors) - with full details
        var instructors = new List<Instructor>
        {
            new() { Name = "Dr. Ahmed Hassan", Email = "ahmed.hassan@nct.edu.eg", PhoneNumber = "01001234567", DepartmentId = departments[0].Id },
            new() { Name = "Dr. Sara Mohamed", Email = "sara.mohamed@nct.edu.eg", PhoneNumber = "01012345678", DepartmentId = departments[1].Id },
            new() { Name = "Dr. Omar Khaled", Email = "omar.khaled@nct.edu.eg", PhoneNumber = "01023456789", DepartmentId = departments[2].Id },
            new() { Name = "Dr. Fatma Ali", Email = "fatma.ali@nct.edu.eg", PhoneNumber = "01034567890", DepartmentId = departments[3].Id },
            new() { Name = "Dr. Mohamed Ibrahim", Email = "mohamed.ibrahim@nct.edu.eg", PhoneNumber = "01045678901", DepartmentId = departments[0].Id },
            new() { Name = "Dr. Nour Hassan", Email = "nour.hassan@nct.edu.eg", PhoneNumber = "01056789012", DepartmentId = departments[1].Id },
            new() { Name = "Dr. Ali Mahmoud", Email = "ali.mahmoud@nct.edu.eg", PhoneNumber = "01067890123", DepartmentId = departments[2].Id },
            new() { Name = "Dr. Hana Salem", Email = "hana.salem@nct.edu.eg", PhoneNumber = "01078901234", DepartmentId = departments[3].Id }
        };
        context.Instructors.AddRange(instructors);
        context.SaveChanges();
        
        // Link instructor user to instructor record
        var instructorUser = context.Users.FirstOrDefault(u => u.Username == "instructor1");
        if (instructorUser != null)
        {
            instructorUser.InstructorId = instructors[0].Id;
            context.SaveChanges();
        }
        
        // Link student user to student record
        var studentUser = context.Users.FirstOrDefault(u => u.Username == "student1");
        if (studentUser != null && students.Any())
        {
            studentUser.StudentId = students[0].Id;
            context.SaveChanges();
        }
        
        // Seed CourseInstructors
        var courseInstructors = new List<CourseInstructor>();
        for (int i = 0; i < courses.Count; i++)
        {
            courseInstructors.Add(new CourseInstructor
            {
                InstructorId = instructors[i % instructors.Count].Id,
                CourseId = courses[i].Id,
                AcademicYear = "2024-2025"
            });
        }
        context.CourseInstructors.AddRange(courseInstructors);
        context.SaveChanges();
        
        // Seed Exams
        var exams = new List<Exam>();
        foreach (var course in courses.Take(8))
        {
            exams.Add(new Exam
            {
                CourseId = course.Id,
                Name = $"{course.Name} Midterm",
                Type = ExamType.Midterm,
                MaxScore = 30,
                Weight = 30,
                ExamDate = DateTime.Now.AddDays(random.Next(10, 30)),
                Room = $"Hall {random.Next(1, 5)}",
                DurationMinutes = 90
            });
            exams.Add(new Exam
            {
                CourseId = course.Id,
                Name = $"{course.Name} Final",
                Type = ExamType.Final,
                MaxScore = 50,
                Weight = 50,
                ExamDate = DateTime.Now.AddDays(random.Next(40, 60)),
                Room = $"Hall {random.Next(1, 5)}",
                DurationMinutes = 180
            });
        }
        context.Exams.AddRange(exams);
        context.SaveChanges();
        
        // Seed Announcements
        var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
        var announcements = new List<Announcement>
        {
            new()
            {
                Title = "Welcome to NCT University!",
                Content = "Welcome to the new academic year 2024-2025. We wish all students success in their studies.",
                Priority = AnnouncementPriority.Important,
                PublishDate = DateTime.Now.AddDays(-5),
                ExpiryDate = DateTime.Now.AddDays(30),
                IsActive = true,
                CreatedByUserId = adminUser?.Id
            },
            new()
            {
                Title = "Midterm Exams Schedule",
                Content = "Midterm examinations will begin on December 20th. Please check your course schedules for exact times.",
                Priority = AnnouncementPriority.Urgent,
                PublishDate = DateTime.Now.AddDays(-2),
                ExpiryDate = DateTime.Now.AddDays(15),
                IsActive = true,
                CreatedByUserId = adminUser?.Id
            },
            new()
            {
                Title = "Library Hours Extended",
                Content = "The university library will remain open until 10 PM during exam period.",
                Priority = AnnouncementPriority.Normal,
                PublishDate = DateTime.Now.AddDays(-1),
                ExpiryDate = DateTime.Now.AddDays(20),
                IsActive = true,
                CreatedByUserId = adminUser?.Id
            }
        };
        context.Announcements.AddRange(announcements);
        context.SaveChanges();
    }
    
    private static void SeedUsers(UniversityDbContext context)
    {
        if (context.Users.Any()) return;
        
        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                PasswordHash = AuthenticationService.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true
            },
            new()
            {
                Username = "student1",
                PasswordHash = AuthenticationService.HashPassword("student123"),
                Role = "Student",
                IsActive = true
            },
            new()
            {
                Username = "instructor1",
                PasswordHash = AuthenticationService.HashPassword("instructor123"),
                Role = "Instructor",
                IsActive = true
            }
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
