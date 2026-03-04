using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Infrastructure.Data.Context;

/// <summary>
/// Seeds initial / master data. Call <see cref="SeedAsync"/> at startup.
/// Schema creation is handled entirely by EF Core migrations.
/// </summary>
public static class Learn2CodeDbContextSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context    = scope.ServiceProvider.GetRequiredService<Learn2CodeDbContext>();
        var logger     = scope.ServiceProvider.GetRequiredService<ILogger<Learn2CodeDbContext>>();

        try
        {
            await ResetSchemaIfNeededAsync(context, logger);
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context, logger);
            await SeedAdminAccountAsync(context, logger);
            await SeedCourseCategoriesAsync(context, logger);
            await SeedDevAccountsAsync(context, logger);
            await SeedCoursesAsync(context, logger);
            await SeedSectionsAsync(context, logger);
            await SeedLessonsAsync(context, logger);
            await SeedExercisesAsync(context, logger);
            await SeedQuizzesAsync(context, logger);
            await SeedCourseCompletionRulesAsync(context, logger);
            await SeedCertificateTemplatesAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    // ─────────────────────────────────────────────
    // If any table already exists (e.g. DB was created manually),
    // drop the entire public schema and recreate it so MigrateAsync
    // starts from a clean slate without conflicts.
    // ─────────────────────────────────────────────
    private static async Task ResetSchemaIfNeededAsync(Learn2CodeDbContext context, ILogger logger)
    {
        var conn = context.Database.GetDbConnection();
        var needsClose = conn.State != System.Data.ConnectionState.Open;
        if (needsClose) await conn.OpenAsync();

        try
        {
            long tableCount;
            await using (var cmd = conn.CreateCommand())
            {
                // COUNT is safer than EXISTS bool cast across driver versions
                cmd.CommandText = """
                    SELECT COUNT(1) FROM pg_catalog.pg_tables
                    WHERE schemaname = 'public' AND tablename = 'accounts'
                """;
                tableCount = Convert.ToInt64(await cmd.ExecuteScalarAsync() ?? 0L);
            }

            if (tableCount > 0)
            {
                logger.LogWarning("Existing tables detected — dropping public schema and recreating for clean migration");
                await using var drop = conn.CreateCommand();
                drop.CommandText = "DROP SCHEMA public CASCADE; CREATE SCHEMA public; GRANT ALL ON SCHEMA public TO PUBLIC;";
                await drop.ExecuteNonQueryAsync();
                logger.LogInformation("Schema reset complete — migrations will now run on a clean database");
            }
            else
            {
                logger.LogInformation("No existing tables found — migrations will run on empty database");
            }
        }
        finally
        {
            if (needsClose) await conn.CloseAsync();
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
        if (await context.CourseCategories.AnyAsync()) return;

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
            context.CourseCategories.Add(new CourseCategory
            {
                CategoryId  = Guid.NewGuid(),
                Name        = c.Name,
                Description = c.Description,
                IsActive    = true,
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            });
            logger.LogInformation("Seeded category: {Category}", c.Name);
        }

        await context.SaveChangesAsync();
    }

    // 
    // Dev / test accounts  (password: Admin@123)
    // 
    private static async Task SeedDevAccountsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        const string hash = "$2a$11$K.W4dRdBcELaXXQrD4z.xOCBPjHxJLZfvFZEVBYfkb7JcbYYXLdGC";

        var devAccounts = new[]
        {
            new { Username = "student_dev",    Email = "student@learn2code.com",    Name = "Dev Student",    Role = "Student"    },
            new { Username = "instructor_dev", Email = "instructor@learn2code.com", Name = "Dev Instructor", Role = "Instructor" }
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
                Password  = hash,
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

        if (instructor == null)
        {
            logger.LogWarning("instructor_dev not found  skipping course seed");
            return;
        }

        var courses = new[]
        {
            new Course
            {
                CourseId     = Guid.NewGuid(),
                Title        = "C# Full-Stack Web Development",
                Description  = "Master ASP.NET Core, Entity Framework, and React to build modern full-stack apps.",
                Difficulty   = CourseDifficulty.Intermediate,
                IsActive     = true,
                InstructorId = instructor.AccountId,
                CategoryId   = webCat?.CategoryId,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            },
            new Course
            {
                CourseId     = Guid.NewGuid(),
                Title        = "Python for Data Science & AI",
                Description  = "Learn Python, Pandas, Numpy, Scikit-learn and build real ML models.",
                Difficulty   = CourseDifficulty.Beginner,
                IsActive     = true,
                InstructorId = instructor.AccountId,
                CategoryId   = dsCat?.CategoryId,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            },
            new Course
            {
                CourseId     = Guid.NewGuid(),
                Title        = "JavaScript & TypeScript Fundamentals",
                Description  = "From zero to hero  variables, functions, async/await, TypeScript types and more.",
                Difficulty   = CourseDifficulty.Beginner,
                IsActive     = true,
                InstructorId = instructor.AccountId,
                CategoryId   = progCat?.CategoryId,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            }
        };

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} courses", courses.Length);
    }

    // 
    // Sections  (3 per course)
    // 
    private static async Task SeedSectionsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Sections.AnyAsync()) return;

        var courses = await context.Courses.ToListAsync();
        if (!courses.Any()) return;

        foreach (var course in courses)
        {
            context.Sections.AddRange(
                new Section { SectionId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Getting Started", OrderNumber = 1, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Section { SectionId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Core Concepts",   OrderNumber = 2, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Section { SectionId = Guid.NewGuid(), CourseId = course.CourseId, Title = "Advanced Topics", OrderNumber = 3, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded sections");
    }

    // 
    // Lessons  (2 per section)
    // 
    private static async Task SeedLessonsAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Lessons.AnyAsync()) return;

        var sections = await context.Sections.ToListAsync();
        if (!sections.Any()) return;

        foreach (var section in sections)
        {
            context.Lessons.AddRange(
                new Lesson { LessonId = Guid.NewGuid(), SectionId = section.SectionId, Title = $"{section.Title} - Lesson 1", OrderNumber = 1, IsFreePreview = section.OrderNumber == 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Lesson { LessonId = Guid.NewGuid(), SectionId = section.SectionId, Title = $"{section.Title} - Lesson 2", OrderNumber = 2, IsFreePreview = false,                     CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded lessons");
    }

    // 
    // Exercises  (1 per lesson)
    // 
    private static async Task SeedExercisesAsync(Learn2CodeDbContext context, ILogger logger)
    {
        if (await context.Exercises.AnyAsync()) return;

        var lessons = await context.Lessons.ToListAsync();
        if (!lessons.Any()) return;

        foreach (var lesson in lessons)
        {
            var exercise = new Exercise
            {
                ExerciseId   = Guid.NewGuid(),
                LessonId     = lesson.LessonId,
                OrderNumber  = 1,
                ExerciseType = ExerciseType.GradedCode,
                Narrative    = $"Practice exercise for: {lesson.Title}. Write a function that returns the sum of two numbers.",
                Language     = "csharp",
                StarterCode  = "public int Add(int a, int b)\n{\n    // TODO: implement\n    return 0;\n}",
                SolutionCode = "public int Add(int a, int b)\n{\n    return a + b;\n}",
                Instruction  = "Implement the Add method to return the sum of a and b.",
                Hint         = "Use the + operator.",
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            };

            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();

            // TestCase has no Input field  only ExpectedOutput
            context.TestCases.AddRange(
                new TestCase { TestCaseId = Guid.NewGuid(), ExerciseId = exercise.ExerciseId, ExpectedOutput = "3",  IsHidden = false, Weight = 0.5m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new TestCase { TestCaseId = Guid.NewGuid(), ExerciseId = exercise.ExerciseId, ExpectedOutput = "30", IsHidden = false, Weight = 0.5m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new TestCase { TestCaseId = Guid.NewGuid(), ExerciseId = exercise.ExerciseId, ExpectedOutput = "0",  IsHidden = true,  Weight = 1.0m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded exercises & test cases");
    }

    // 
    // Quizzes & Options  (1 quiz per lesson)
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
                QuizId      = Guid.NewGuid(),
                LessonId    = lesson.LessonId,
                OrderNumber = 1,
                Question    = $"What is the main purpose of {lesson.Title}?",
                Explanation = "This question tests your understanding of the lesson's core objective.",
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            };

            context.Quizzes.Add(quiz);
            await context.SaveChangesAsync();

            context.QuizOptions.AddRange(
                new QuizOption { OptionId = Guid.NewGuid(), QuizId = quiz.QuizId, Content = "To learn fundamentals",       IsCorrect = true,  CreatedAt = DateTime.UtcNow },
                new QuizOption { OptionId = Guid.NewGuid(), QuizId = quiz.QuizId, Content = "To practice advanced topics", IsCorrect = false, CreatedAt = DateTime.UtcNow },
                new QuizOption { OptionId = Guid.NewGuid(), QuizId = quiz.QuizId, Content = "To review previous material", IsCorrect = false, CreatedAt = DateTime.UtcNow },
                new QuizOption { OptionId = Guid.NewGuid(), QuizId = quiz.QuizId, Content = "None of the above",           IsCorrect = false, CreatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Seeded quizzes & options");
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
                RuleId                  = Guid.NewGuid(),
                CourseId                = course.CourseId,
                MinLessonCompletionPct  = 80m,
                MinExercisePassPct      = 70m,
                MinSectionQuizScore     = 60m,
                RequireAllSectionQuiz   = true,
                CreatedAt               = DateTime.UtcNow,
                UpdatedAt               = DateTime.UtcNow
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
                TemplateId    = Guid.NewGuid(),
                CourseId      = course.CourseId,
                Title         = $"Certificate of Completion - {course.Title}",
                Description   = "This certificate is awarded upon successful completion of the course.",
                SignatureName = "Learn2Code Team",
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded certificate templates");
    }
}
