using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Learn2Code.Infrastructure.Data.Context;

public class Learn2CodeDbContext : DbContext
{
    public Learn2CodeDbContext(DbContextOptions<Learn2CodeDbContext> options) : base(options)
    {
    }

    public Learn2CodeDbContext()
    {
    }

    public static string GetConnectionString(string connectionStringName)
    {
        string? envConnectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{connectionStringName}");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            return envConnectionString;
        }

        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var currentDir = new DirectoryInfo(basePath);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "Learn2Code.API", "appsettings.json")))
        {
            currentDir = currentDir.Parent;
        }

        var configPath = currentDir != null
            ? Path.Combine(currentDir.FullName, "Learn2Code.API")
            : basePath;

        var config = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return config.GetConnectionString(connectionStringName) ?? string.Empty;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<AccountRole> AccountRoles { get; set; } = null!;
    public DbSet<CourseCategory> CourseCategories { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<TestCase> TestCases { get; set; } = null!;
    public DbSet<Submission> Submissions { get; set; } = null!;
    public DbSet<SubmissionResult> SubmissionResults { get; set; } = null!;
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<QuizOption> QuizOptions { get; set; } = null!;
    public DbSet<QuizSubmission> QuizSubmissions { get; set; } = null!;
    public DbSet<FinalTest> FinalTests { get; set; } = null!;
    public DbSet<Progress> Progresses { get; set; } = null!;
    public DbSet<CertificateTemplate> CertificateTemplates { get; set; } = null!;
    public DbSet<Certification> Certifications { get; set; } = null!;
    public DbSet<CourseCompletionRule> CourseCompletionRules { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite key for AccountRole
        modelBuilder.Entity<AccountRole>()
            .HasKey(ar => new { ar.AccountId, ar.RoleId });

        // Configure unique constraints
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<CourseCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Certification>()
            .HasIndex(c => c.CertificateCode)
            .IsUnique();

        // Configure one-to-one relationships
        modelBuilder.Entity<Course>()
            .HasOne(c => c.FinalTest)
            .WithOne(f => f.Course)
            .HasForeignKey<FinalTest>(f => f.CourseId);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.CertificateTemplate)
            .WithOne(ct => ct.Course)
            .HasForeignKey<CertificateTemplate>(ct => ct.CourseId);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.CompletionRule)
            .WithOne(cr => cr.Course)
            .HasForeignKey<CourseCompletionRule>(cr => cr.CourseId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Payment)
            .WithOne(p => p.Enrollment)
            .HasForeignKey<Payment>(p => p.EnrollmentId);

        // Configure enums to be stored as strings in PostgreSQL
        modelBuilder.Entity<Course>()
            .Property(c => c.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentMethod)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Document>()
            .Property(d => d.DocType)
            .HasConversion<string>();

        modelBuilder.Entity<Exercise>()
            .Property(e => e.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<Exercise>()
            .Property(e => e.Language)
            .HasConversion<string>();

        modelBuilder.Entity<Submission>()
            .Property(s => s.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Progress>()
            .Property(p => p.Status)
            .HasConversion<string>();

        // Configure decimal precision
        modelBuilder.Entity<Course>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Course>()
            .Property(c => c.OriginalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.ProgressPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TestCase>()
            .Property(t => t.Weight)
            .HasPrecision(5, 2);

        modelBuilder.Entity<FinalTest>()
            .Property(f => f.PassingScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinLessonCompletionPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinExercisePassPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinQuizScore)
            .HasPrecision(5, 2);
    }
}
