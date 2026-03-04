using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Infrastructure.Data.Context;

/// <summary>
/// Seeds initial / master data. Call <see cref="SeedAsync"/> at startup.
/// Schema creation is handled entirely by EF Core migrations  no DDL here.
/// </summary>
public static class Learn2CodeDbContextSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context     = scope.ServiceProvider.GetRequiredService<Learn2CodeDbContext>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILogger<Learn2CodeDbContext>>();

        try
        {
            // Apply any pending EF migrations (creates / alters schema as needed)
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context, logger);
            await SeedAdminAccountAsync(context, logger);
            await SeedCourseCategoriesAsync(context, logger);
            await SeedDevAccountsAsync(context, logger);
            await SeedCoursesAsync(context, logger);
            await SeedSectionsAndLessonsAsync(context, logger);
            await SeedDocumentsAsync(context, logger);
            await SeedExercisesAndTestCasesAsync(context, logger);
            await SeedQuizzesAsync(context, logger);
            await SeedFinalTestsAsync(context, logger);
            await SeedCourseCompletionRulesAsync(context, logger);
            await SeedCertificateTemplatesAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    // 
    // Roles
    // 
    private static async Task SeedRolesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        var roles = new[]
        {
            new { Name = "Admin",      Description = "System Administrator with full access" },
            new { Name = "Student",    Description = "Learner who can enroll in courses" },
            new { Name = "Instructor", Description = "Teacher who can create and manage courses" }
        };

        foreach (var r in roles)
        {
            if (!await context.Roles.AnyAsync(x => x.RoleName == r.Name))
            {
                context.Roles.Add(new Role
                {
                    RoleId      = Guid.NewGuid(),
                    RoleName    = r.Name,
                    Description = r.Description,
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                });
                logger.LogInformation("Seeded role: {Role}", r.Name);
            }
        }

        await context.SaveChangesAsync();
    }

    // 
    // Admin account  (password: Admin@123)
    // 
    private static async Task SeedAdminAccountAsync(Learn2CodeDbContext context, ILogger logger)
    {
        const string username       = "admin";
        const string hashedPassword = "$2a$11$K.W4dRdBcELaXXQrD4z.xOCBPjHxJLZfvFZEVBYfkb7JcbYYXLdGC";

        if (await context.Accounts.AnyAsync(a => a.Username == username))
            return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
        if (adminRole == null)
        {
            logger.LogWarning("Admin role not found  skipping admin account seed");
            return;
        }

        var admin = new Account
        {
            AccountId = Guid.NewGuid(),
            Username  = username,
            Email     = "admin@learn2code.com",
            Password  = hashedPassword,
            Name      = "System Admin",
            IsActive  = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(admin);
        await context.SaveChangesAsync();

        context.AccountRoles.Add(new AccountRole
        {
            AccountId  = admin.AccountId,
            RoleId     = adminRole.RoleId,
            AssignedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded admin account: {Username}", username);
    }

    // 
    // Course categories
    // 
    private static async Task SeedCourseCategoriesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        var categories = new[]
        {
            new { Name = "Web Development",       Description = "Frontend, Backend, Full-stack web development" },
            new { Name = "Mobile Development",    Description = "iOS, Android, React Native, Flutter" },
            new { Name = "Data Science",          Description = "Python, Machine Learning, AI" },
            new { Name = "DevOps",                Description = "Docker, Kubernetes, CI/CD" },
            new { Name = "Programming Languages", Description = "C#, Java, Python, JavaScript fundamentals" }
        };

        foreach (var c in categories)
        {
            if (!await context.CourseCategories.AnyAsync(x => x.Name == c.Name))
            {
                context.CourseCategories.Add(new CourseCategory
                {
                    Name        = c.Name,
                    Description = c.Description,
                    CreatedAt   = DateTime.UtcNow
                });
                logger.LogInformation("Seeded category: {Category}", c.Name);
            }
        }

        await context.SaveChangesAsync();
    }

    // 
    // Dev / test accounts  (password: Admin@123)
    // Remove or guard this in production
    // 
    private static async Task SeedDevAccountsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        const string hash = "$2a$11$K.W4dRdBcELaXXQrD4z.xOCBPjHxJLZfvFZEVBYfkb7JcbYYXLdGC";

        var devAccounts = new[]
        {
            new { Username = "student_dev",    Email = "student@learn2code.com",    Name = "Dev Student",    Role = "Student",    Password = hash },
            new { Username = "instructor_dev", Email = "instructor@learn2code.com", Name = "Dev Instructor", Role = "Instructor", Password = hash }
        };

        foreach (var dev in devAccounts)
        {
            if (await context.Accounts.AnyAsync(a => a.Username == dev.Username))
                continue;

            var role = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == dev.Role);
            if (role == null) continue;

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Username  = dev.Username,
                Email     = dev.Email,
                Password  = dev.Password,
                Name      = dev.Name,
                IsActive  = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            context.AccountRoles.Add(new AccountRole
            {
                AccountId  = account.AccountId,
                RoleId     = role.RoleId,
                AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded dev account: {Username} ({Role})", dev.Username, dev.Role);
        }
    }

    // 
    // Courses
    // 
    private static async Task SeedCoursesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Courses.AnyAsync()) return;

        var instructor = await context.Accounts.FirstOrDefaultAsync(a => a.Username == "instructor_dev");
        var webCat     = await context.CourseCategories.FirstOrDefaultAsync(c => c.Name == "Web Development");
        var progCat    = await context.CourseCategories.FirstOrDefaultAsync(c => c.Name == "Programming Languages");
        var dsCat      = await context.CourseCategories.FirstOrDefaultAsync(c => c.Name == "Data Science");

        var courses = new[]
        {
            new Course
            {
                Title         = "C# Full-Stack Web Development",
                Description   = "Master ASP.NET Core, Entity Framework, and React to build modern full-stack apps.",
                Price         = 499000,
                OriginalPrice = 799000,
                Difficulty    = CourseDifficulty.Intermediate,
                IsPublished   = true,
                AdminId       = instructor?.AccountId,
                CategoryId    = webCat?.Id,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            },
            new Course
            {
                Title         = "Python for Data Science & AI",
                Description   = "Learn Python, Pandas, Numpy, Scikit-learn and build real ML models.",
                Price         = 599000,
                OriginalPrice = 999000,
                Difficulty    = CourseDifficulty.Beginner,
                IsPublished   = true,
                AdminId       = instructor?.AccountId,
                CategoryId    = dsCat?.Id,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            },
            new Course
            {
                Title         = "JavaScript & TypeScript Fundamentals",
                Description   = "From zero to hero  variables, functions, async/await, TypeScript types and more.",
                Price         = 0,
                OriginalPrice = 0,
                Difficulty    = CourseDifficulty.Beginner,
                IsPublished   = true,
                AdminId       = instructor?.AccountId,
                CategoryId    = progCat?.Id,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            }
        };

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} courses", courses.Length);
    }

    // 
    // Sections & Lessons
    // 
    private static async Task SeedSectionsAndLessonsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Sections.AnyAsync()) return;

        var courses = await context.Courses.ToListAsync();
        if (!courses.Any()) return;

        foreach (var course in courses)
        {
            var sections = new[]
            {
                new Section { CourseId = course.Id, Title = "Getting Started", OrderNumber = 1, IsFreePreview = true,  CreatedAt = DateTime.UtcNow },
                new Section { CourseId = course.Id, Title = "Core Concepts",   OrderNumber = 2, IsFreePreview = false, CreatedAt = DateTime.UtcNow },
                new Section { CourseId = course.Id, Title = "Advanced Topics", OrderNumber = 3, IsFreePreview = false, CreatedAt = DateTime.UtcNow }
            };

            context.Sections.AddRange(sections);
            await context.SaveChangesAsync();

            foreach (var section in sections)
            {
                context.Lessons.AddRange(
                    new Lesson { SectionId = section.Id, Title = $"{section.Title} - Lesson 1", OrderNumber = 1, IsPreviewable = section.IsFreePreview, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Lesson { SectionId = section.Id, Title = $"{section.Title} - Lesson 2", OrderNumber = 2, IsPreviewable = false,                 CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                );
            }
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded sections & lessons");
    }

    // 
    // Documents
    // 
    private static async Task SeedDocumentsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Documents.AnyAsync()) return;

        var lessons = await context.Lessons.ToListAsync();
        if (!lessons.Any()) return;

        foreach (var lesson in lessons)
        {
            context.Documents.Add(new Document
            {
                LessonId    = lesson.Id,
                Title       = $"{lesson.Title} - Reading Material",
                Content     = $"# {lesson.Title}\n\nThis is the reading material for **{lesson.Title}**.\n\nReplace this with actual content.",
                DocType     = DocumentType.Theory,
                OrderNumber = 1,
                CreatedAt   = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded documents");
    }

    // 
    // Exercises & Test Cases
    // 
    private static async Task SeedExercisesAndTestCasesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Exercises.AnyAsync()) return;

        var lessons = await context.Lessons.ToListAsync();
        if (!lessons.Any()) return;

        foreach (var lesson in lessons)
        {
            var exercise = new Exercise
            {
                LessonId     = lesson.Id,
                Title        = $"Exercise: {lesson.Title}",
                Description  = "Write a function that returns the sum of two numbers.",
                Difficulty   = ExerciseDifficulty.Easy,
                Language     = ProgrammingLanguage.CSharp,
                StarterCode  = "public int Add(int a, int b)\n{\n    // TODO: implement\n    return 0;\n}",
                SolutionCode = "public int Add(int a, int b)\n{\n    return a + b;\n}",
                CreatedAt    = DateTime.UtcNow
            };
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();

            context.TestCases.AddRange(
                new TestCase { ExerciseId = exercise.Id, Input = "1 2",   ExpectedOutput = "3",  IsHidden = false, Weight = 0.5m },
                new TestCase { ExerciseId = exercise.Id, Input = "10 20", ExpectedOutput = "30", IsHidden = false, Weight = 0.5m },
                new TestCase { ExerciseId = exercise.Id, Input = "-5 5",  ExpectedOutput = "0",  IsHidden = true,  Weight = 1.0m }
            );
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded exercises & test cases");
    }

    // 
    // Quizzes & Options
    // 
    private static async Task SeedQuizzesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Quizzes.AnyAsync()) return;

        var lessons = await context.Lessons.ToListAsync();
        if (!lessons.Any()) return;

        foreach (var lesson in lessons)
        {
            var quiz = new Quiz
            {
                LessonId    = lesson.Id,
                Question    = $"What is the main purpose of {lesson.Title}?",
                Explanation = "This question tests your understanding of the lesson's core objective."
            };
            context.Quizzes.Add(quiz);
            await context.SaveChangesAsync();

            context.QuizOptions.AddRange(
                new QuizOption { QuizId = quiz.Id, Content = "To learn fundamentals",       IsCorrect = true  },
                new QuizOption { QuizId = quiz.Id, Content = "To practice advanced topics", IsCorrect = false },
                new QuizOption { QuizId = quiz.Id, Content = "To review previous material", IsCorrect = false },
                new QuizOption { QuizId = quiz.Id, Content = "None of the above",           IsCorrect = false }
            );
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded quizzes & options");
    }

    // 
    // Final Tests  (1 per course)
    // 
    private static async Task SeedFinalTestsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.FinalTests.AnyAsync()) return;

        var courses = await context.Courses.ToListAsync();
        foreach (var course in courses)
        {
            context.FinalTests.Add(new FinalTest
            {
                CourseId        = course.Id,
                Title           = $"Final Test - {course.Title}",
                Description     = "Comprehensive assessment covering all topics in this course.",
                DurationMinutes = 60,
                PassingScore    = 70m
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded final tests");
    }

    // 
    // Course Completion Rules  (1 per course)
    // 
    private static async Task SeedCourseCompletionRulesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.CourseCompletionRules.AnyAsync()) return;

        var courses = await context.Courses.ToListAsync();
        foreach (var course in courses)
        {
            context.CourseCompletionRules.Add(new CourseCompletionRule
            {
                CourseId                   = course.Id,
                MinLessonCompletionPercent = 80m,
                MinExercisePassPercent     = 70m,
                MinQuizScore               = 60m,
                RequireFinalTest           = true
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded course completion rules");
    }

    // 
    // Certificate Templates  (1 per course)
    // 
    private static async Task SeedCertificateTemplatesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.CertificateTemplates.AnyAsync()) return;

        var courses = await context.Courses.ToListAsync();
        foreach (var course in courses)
        {
            context.CertificateTemplates.Add(new CertificateTemplate
            {
                CourseId      = course.Id,
                Title         = $"Certificate of Completion - {course.Title}",
                Description   = "This certificate is awarded upon successful completion of the course.",
                SignatureName = "Learn2Code Team",
                CreatedAt     = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded certificate templates");
    }
}
